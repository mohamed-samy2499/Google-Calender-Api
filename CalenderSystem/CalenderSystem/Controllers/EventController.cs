using AutoMapper;
using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.Feature.Events.Commands.Models;
using CalenderSystem.Application.Feature.Events.Commands.Validators;
using CalenderSystem.Application.Feature.Events.Queries.Models;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CalenderSystem.Api.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CalenderSystem.Api.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EventController : ControllerBase
    {
        #region Ctor
        private readonly IMediator _mediator;
        private readonly IEventService _eventService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly string? clientId = null!;
        private readonly string? authUri = null!;
        private readonly string? clientSecret = null!;
        private readonly string? redirectUri = null!;
        private readonly string? tokenUri = null!;
        private readonly string? apiKey = null!;

        public EventController(IEventService eventService, IConfiguration configuration,
            UserManager<ApplicationUser> userManager, IMediator mediator, IMapper mapper,
            AppDbContext dbContext)
        {
            _mediator = mediator;
            _eventService = eventService;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
            _httpClient = new HttpClient();
            clientId = _configuration["Authentication:Google:client_id"];
            authUri = _configuration["Authentication:Google:auth_uri"];
            clientSecret = _configuration["Authentication:Google:client_secret"];
            redirectUri = _configuration["Authentication:Google:redirect_uri"];
            tokenUri = _configuration["Authentication:Google:token_uri"];
            apiKey = _configuration["Authentication:Google:api_key"];
        }
        #endregion

        [HttpGet("get-allGoogleCalendarEvents")]
        public async Task<ActionResult> GetAllCalendarEvents([FromQuery] DateTime? startDate,
                                                             [FromQuery] DateTime? endDate,
                                                             [FromQuery] string? searchQuery,
                                                             [FromQuery] int page = 1,
                                                             [FromQuery] int pageSize = 10)
        {
            try
            {

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }
                var response = await _eventService.GetAllGoogleCalendarEvent(user,
                    clientId, clientSecret,
                    startDate,endDate,searchQuery,page,pageSize);
                if (response != null )
                {
                    return Ok(response);
                }
                else
                {
                    // Handle the error
                    return BadRequest("failed to fetch the events");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-allDatabaseEvents")]
        public async Task<ActionResult> GetAllEvents()
        {
            try
            {
                var result = await _mediator.Send(new GetEventListQuery());
                if(result.Succeeded)
                    return Ok(result);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-event")]
        public async Task<ActionResult> GetEvent([FromQuery] GetEventByIdQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                if (result.Succeeded)
                    return Ok(result);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-event")]
        public async Task<ActionResult> AddEvent([FromBody] AddEventDTO eventDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return BadRequest("User not found.");



                var validationResult = await EventHelper.ValidateAddEventAsync(eventDto, user);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var response = await _eventService.AddGoogleCalendarEvent(eventDto, user, clientId, clientSecret);
                if (string.IsNullOrEmpty(response))
                    return BadRequest("Failed to add the event to Google Calendar");

                var addEventCommand = EventHelper.CreateAddEventCommand(eventDto, user, response);
                var result = await _mediator.Send(addEventCommand);

                if (!result.Succeeded)
                {
                    var deleteEventFromCalendarResult = await _eventService.DeleteGoogleCalendarEvent(response, user, clientId, clientSecret);
                    return BadRequest(result.Message);
                }

                return Ok(addEventCommand);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("update-event")]
        public async Task<ActionResult> UpdateEvent([FromBody] UpdateEventDTO eventDto)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return BadRequest("User not found.");

                var validationResult = await EventHelper.ValidateUpdateEventAsync(eventDto, user);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var response = await _eventService.UpdateGoogleCalendarEvent(eventDto, user, clientId, clientSecret);
                if (string.IsNullOrEmpty(response))
                    return BadRequest("Failed to update the event in Google Calendar");

                var existingEventResult = await _mediator.Send(new GetEventByIdQuery() { EventId = eventDto.Id });

                if (!existingEventResult.Succeeded)
                    return BadRequest(existingEventResult);

                var existingEvent = existingEventResult.Data;
                var updateEventValidationCheck = EventHelper.CreateUpdateEventCommand(eventDto, user);
                var result = await _mediator.Send(updateEventValidationCheck);

                if (!result.Succeeded)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("delete-event")]
        public async Task<ActionResult> DeleteEvent(int eventId)
        {
            try
            {
                if (eventId <= 0)
                    return BadRequest("Event Id must be greater than 0");

                var existingEventResult = await _mediator.Send(new GetEventByIdQuery() { EventId = eventId });

                if (!existingEventResult.Succeeded)
                    return BadRequest(existingEventResult);

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return BadRequest("User not found.");

                var validationResult = await new DeleteEventValidator().ValidateAsync(new DeleteEventCommand() { EventId = eventId });

                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                var googleCalendarEventId = existingEventResult.Data.GoogleCalendarEventId;
                var result = await _eventService.DeleteGoogleCalendarEvent(googleCalendarEventId, user, clientId, clientSecret);

                if (result)
                {
                    var deleteResult = await _mediator.Send(new DeleteEventCommand() { EventId = eventId });

                    if (deleteResult.Succeeded)
                        return Ok(deleteResult);

                    return BadRequest(deleteResult);
                }

                return BadRequest("Can't delete the Google Calendar event");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
