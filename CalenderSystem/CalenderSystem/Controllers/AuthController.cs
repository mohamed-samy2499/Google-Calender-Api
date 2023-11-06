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

		public AuthController(IConfiguration configuration,IAuthService authService)
		{
			_configuration = configuration;
			_authService = authService;
		}

		[HttpGet("google-login-url")]
		public IActionResult GoogleLogin()
		{
			var clientId = _configuration["Authentication:Google:client_id"];
			var authUri = _configuration["Authentication:Google:auth_uri"];
			var clientSecret = _configuration["Authentication:Google:client_secret"];
			var redirectUri = _configuration["Authentication:Google:redirect_uri"];


			// Construct the Google login URL with the appropriate parameters
			var googleLoginUrl = _authService.GetAuthCode(authUri,redirectUri,clientId);

			return Redirect(googleLoginUrl);
		}

		[HttpGet("callback")]
		public void GoogleLoginCallback(string code,string error,string state)
		{
			if(string.IsNullOrEmpty(error))
			{

			 Ok();
			}

		}

	}
}
