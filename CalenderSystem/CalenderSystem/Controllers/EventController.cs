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
        private readonly string? clientId = null!;
        private readonly string? authUri = null!;
        private readonly string? clientSecret = null!;
        private readonly string? redirectUri = null!;
        private readonly string? tokenUri = null!;
        EventController(IEventService eventService, IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _eventService = eventService;
            _configuration = configuration;
            _userManager = userManager;
            clientId = _configuration["Authentication:Google:client_id"];
            authUri = _configuration["Authentication:Google:auth_uri"];
            clientSecret = _configuration["Authentication:Google:client_secret"];
            redirectUri = _configuration["Authentication:Google:redirect_uri"];
            tokenUri = _configuration["Authentication:Google:token_uri"];
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
        //[HttpPost("add-event")]
        //public async Task<ActionResult> AddEvent([FromBody] AddEventDTO eventDto)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //}
    }
}
