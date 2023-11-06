using CalenderSystem.Application.DTOs;
using CalenderSystem.Application.IServices;
using CalenderSystem.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CalenderSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly IAuthService _authService;
		private readonly string? clientId = null!;
		private readonly string? authUri = null!;
		private readonly string? clientSecret = null!;
		private readonly string? redirectUri = null!;
		private readonly string? tokenUri = null!;

		public AuthController(IConfiguration configuration,IAuthService authService)
		{
			_configuration = configuration;
			_authService = authService;
			clientId = _configuration["Authentication:Google:client_id"];
			authUri = _configuration["Authentication:Google:auth_uri"];
			clientSecret = _configuration["Authentication:Google:client_secret"];
			redirectUri = _configuration["Authentication:Google:redirect_uri"];
			tokenUri = _configuration["Authentication:Google:token_uri"];

		}

		[HttpGet("google-login-url")]
		public IActionResult GoogleLogin()
		{

			var googleLoginUrl = _authService.GetAuthCode(authUri,redirectUri,clientId);

            return Ok(new { GoogleLoginUrl = googleLoginUrl });
        }
		//this endpoint is for call back after the user sign in using google
		[HttpGet("callback")]
		public async void GoogleLoginCallback()
		{
			string? code = HttpContext.Request.Query["code"];
			string? scope = HttpContext.Request.Query["scope"];
			if (!string.IsNullOrEmpty(code))
			{
				GoogleTokenResponseDTO  tokenRes = await  _authService
					.GetTokens(code, redirectUri, clientId, clientSecret, tokenUri);
				if (tokenRes != null) { }
			}

		}

	}
}
