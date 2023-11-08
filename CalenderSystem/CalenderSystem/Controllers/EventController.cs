using AutoMapper;
using Azure.Core;
using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.Feature.Events.Commands.Models;
using CalenderSystem.Application.Feature.Events.Commands.Validators;
using CalenderSystem.Application.Feature.Events.Queries.Models;
using CalenderSystem.Application.Feature.Events.Queries.Response;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities;
using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Infrastructure.Database;
using Google.Apis.Auth.OAuth2;
using IdentityServer4.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CalenderSystem.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

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
                                                             [FromQuery] string? searchQuery)
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
                    startDate,endDate,searchQuery);
                if (response != null )
                {
                    return Ok(response);
                }
                else
                {
                    // Handle the error
                    Console.WriteLine($"Error creating event: {response}");
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
                //get the current logged in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }
                //validate the model input
                var addEventValidationCheck = new AddEventCommand()
                {
                    Summary = eventDto.Summary,
                    Description = eventDto.Description,
                    Location = eventDto.Location,
                    StartDateTime = eventDto.StartTime,
                    EndDateTime = eventDto.EndTime,
                    RefreshToken = user.GoogleRefreshToken,
                    UserId = user.Id
                };
                var validationResult = await new AddEventValidator().ValidateAsync(addEventValidationCheck);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }
                //add the event to google calendar
                var response = await _eventService.AddGoogleCalendarEvent(eventDto, user,
                    clientId, clientSecret);
                if (!string.IsNullOrEmpty(response))
                {
                    //add the event to our database
                    var addEventCommand = new AddEventCommand()
                    {
                        Summary = eventDto.Summary,
                        Description = eventDto.Description,
                        Location = eventDto.Location,
                        StartDateTime = eventDto.StartTime,
                        EndDateTime = eventDto.EndTime,
                        RefreshToken = user.GoogleRefreshToken,
                        GoogleCalendarEventId = response,
                        UserId = user.Id
                    };
                    var result = await _mediator.Send(addEventCommand);
                    if (result.Succeeded)
                            return Ok(addEventCommand);
                    else
                    {
                        var deleteEventFromCalendarResult = await _eventService.DeleteGoogleCalendarEvent(response,
                            user, clientId, clientSecret);
                        return BadRequest(result.Message);
                    }
                }
                else
                {
                    // Handle the error
                    return BadRequest("failed to add the event to google calendar");
                }
                
 
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
                //get the current logged in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }
                //validate the update model
                var updateEventValidationCheck = new UpdateEventCommand()
                {
                    Id = eventDto.Id,
                    Summary = eventDto.Summary,
                    Description = eventDto.Description,
                    Location = eventDto.Location,
                    StartDateTime = eventDto.StartTime,
                    EndDateTime = eventDto.EndTime,
                    GoogleCalendarEventId = eventDto.GoogleCalendarEventId
                };
                var validationResult = await new UpdateEventValidator().ValidateAsync(updateEventValidationCheck);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }
                //update the event in google calendar
                var response = await _eventService.UpdateGoogleCalendarEvent(eventDto, user,
                    clientId, clientSecret);
                if (!string.IsNullOrEmpty(response))
                {
                    //get the existing event in the database
                    var existingEventResult = await _mediator.Send(new GetEventByIdQuery()
                    {
                        EventId = eventDto.Id
                    });
                    if (existingEventResult.Succeeded)
                    {
                        var existingEvent = existingEventResult.Data;
                        //update the event in our database
                        updateEventValidationCheck.UserId = user.Id;
                        var result = await _mediator.Send(updateEventValidationCheck);
                        if (result.Succeeded)
                            return Ok(result);
                        return BadRequest(result);
                    }
                    return BadRequest(existingEventResult);
                }
                else
                {
                    return BadRequest("failed to update the event in google calendar");
                }
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
                if(eventId > 0)
                {
                    //get the existing event in the database
                    var existingEventResult = await _mediator.Send(new GetEventByIdQuery()
                    {
                        EventId = eventId
                    });
                    if (existingEventResult.Succeeded)
                    {
                        var user = await _userManager.GetUserAsync(User);
                        if (user == null)
                        {
                            return BadRequest("User not found.");
                        }
                        //validate the update model
                        var deleteEventValidationCheck = new DeleteEventCommand()
                        {
                            EventId = eventId
                        };
                        var validationResult = await new DeleteEventValidator().ValidateAsync(deleteEventValidationCheck);
                        if (!validationResult.IsValid)
                        {
                            return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                        }
                        var googleCalendarEventId = existingEventResult.Data.GoogleCalendarEventId;
                        var result = await  _eventService.DeleteGoogleCalendarEvent(googleCalendarEventId,
                            user, clientId, clientSecret);
                        if (result)
                        {                            
                            //delete the event in our database
                            var deleteResult = await _mediator.Send(deleteEventValidationCheck);
                            if (deleteResult.Succeeded)
                                return Ok(deleteResult);
                            return BadRequest(deleteResult);
                        }
                        else
                        {
                            return BadRequest("can't delete the google calendar event");
                        }
                        
                    }
                    return BadRequest(existingEventResult);
                }
                return BadRequest("event Id can't be less than or equal to 0");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
