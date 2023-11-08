using Azure.Core;
using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities;
using CalenderSystem.Domain.Entities.Identity;
using Google.Apis.Auth.OAuth2;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace CalenderSystem.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class EventController : ControllerBase
    {
        #region Ctor
        private readonly IEventService _eventService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly string? clientId = null!;
        private readonly string? authUri = null!;
        private readonly string? clientSecret = null!;
        private readonly string? redirectUri = null!;
        private readonly string? tokenUri = null!;
        private readonly string? apiKey = null!;

        public EventController(IEventService eventService, IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _configuration = configuration;
            _userManager = userManager;
            _httpClient = new HttpClient();
            clientId = _configuration["Authentication:Google:client_id"];
            authUri = _configuration["Authentication:Google:auth_uri"];
            clientSecret = _configuration["Authentication:Google:client_secret"];
            redirectUri = _configuration["Authentication:Google:redirect_uri"];
            tokenUri = _configuration["Authentication:Google:token_uri"];
            apiKey = _configuration["Authentication:Google:api_key"];
        }
        #endregion
        [HttpGet]
        public async Task<ActionResult> GetAllEvents()
        {
            try
            {

                var events = await _eventService.GetAllEventsAsync();
                if(events == null)
                {
                    return NoContent();
                }
                return Ok(events);
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
                {
                    return BadRequest("User not found.");
                }
                var response = await _eventService.AddGoogleCalendarEvent(eventDto, user,
                    clientId,clientSecret);
                if (!string.IsNullOrEmpty(response))
                {
                    //map the event to my database
                    var eventDb = new Event()
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
                    var result = await _eventService.AddEventAsync(eventDb);
                    if (result == null)
                        return BadRequest("Event Added to google calendar but wasn't saved in our database");
                    return Ok("event created and added successfully to our db and google calendar");
                }
                else
                {
                    // Handle the error
                    Console.WriteLine($"Error creating event: {response}");
                    return BadRequest("failed to add the event");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
