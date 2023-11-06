using CalenderSystem.Application.Helper;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}
		public async Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info)
		{
			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				// Handle the case where the user doesn't exist in your application.
				// You can create a new user or take appropriate actions.
				// For this example, we create a new user with the email.
				user = new ApplicationUser { UserName = email, Email = email };
				var result = await _userManager.CreateAsync(user);

				if (!result.Succeeded)
				{
					// Handle the case where user creation failed.
					// You may want to return an error message or redirect the user to an error page.
					return null;
				}
			}

			// Sign in the user with the user manager.
			await _signInManager.SignInAsync(user, isPersistent: false);

			return SignInResult.Success;
		}
		public string GetAuthCode(string authUrl, string redirectUrl, string clientId)
		{
			string scopeURL1 = authUrl + "?redirect_uri={0}&prompt={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}";

			var redirectURL = redirectUrl;

			string prompt = "consent";

			string response_type = "code";
			string clientID = clientId;
			string scope = "https://www.googleapis.com/auth/calendar + https://www.googleapis.com/auth/calendar.events";

			string access_type = "offline";
			string redirect_uri_encode = redirectURL;//Method.urlEncodeForGoogle(redirectURL);
			var mainURL = string.Format(scopeURL1, redirect_uri_encode, prompt, response_type, clientID, scope, access_type);

			return mainURL;
		}

		public async Task SignOutAsync()
		{
			await _signInManager.SignOutAsync();
		}
	}
}
