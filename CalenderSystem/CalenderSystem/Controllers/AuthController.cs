using Azure.Core;
using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.IServices;
using CalenderSystem.Application.Services;
using CalenderSystem.Domain.Entities.Identity;
using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CalenderSystem.Api.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
        #region ctor
        private readonly IConfiguration _configuration;
		private readonly IAuthService _authService;
		private readonly IApplicationUserService _applicationUserService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly HttpClient _httpClient;
        private readonly string? clientId = null!;
		private readonly string? authUri = null!;
		private readonly string? clientSecret = null!;
		private readonly string? redirectUri = null!;
		private readonly string? tokenUri = null!;

		public AuthController(IConfiguration configuration, IAuthService authService,
			SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
			IApplicationUserService applicationUserService)
		{
			_configuration = configuration;
			_authService = authService;
			_applicationUserService = applicationUserService;
			_signInManager = signInManager;
			_userManager = userManager;
            _httpClient = new HttpClient();
            clientId = _configuration["Authentication:Google:client_id"];
			authUri = _configuration["Authentication:Google:auth_uri"];
			clientSecret = _configuration["Authentication:Google:client_secret"];
			redirectUri = _configuration["Authentication:Google:redirect_uri"];
			tokenUri = _configuration["Authentication:Google:token_uri"];

		}
        #endregion


        [HttpGet("google-login-url")]
		public IActionResult GoogleLogin()
		{
			try
			{

			var googleLoginUrl = _authService.GetAuthCode(authUri,redirectUri,clientId);

            return Ok(new { GoogleLoginUrl = googleLoginUrl });
			}
			catch(Exception ex)
			{
				return BadRequest(ex.Message);
			}
        }
        //this endpoint is for call back after the user sign in using google
        [AllowAnonymous]
        [HttpGet("callback")]
		public async Task<IActionResult> GoogleLoginCallback()
		{
			try
			{

				string? code = HttpContext.Request.Query["code"];
				string? scope = HttpContext.Request.Query["scope"];
				if (!string.IsNullOrEmpty(code))
				{
					GoogleTokenResponseDTO  tokenRes = await  _authService
						.GetTokens(code, redirectUri, clientId, clientSecret, tokenUri);
                    if (tokenRes != null) 
					{
						//get the email and google id of the user
                        var userInfo = await _authService.GetUserInfoAsync(tokenRes.Access_token);
						
                        
                        if (userInfo == null)
                        {                            
                            return BadRequest("email or googleUserId is null");
                        }

						//add or update the user if it already exists
						var AddUserResult = await _authService.AddOrUpdateUserAsync(tokenRes, userInfo);
						if (AddUserResult != null)
							return AddUserResult ? Ok(tokenRes) 
								: BadRequest("couldn't create or update the user");
					}
					return BadRequest("token is null");
				}
                return BadRequest("code is null");

            }
            catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}

	}
}
