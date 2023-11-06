﻿using CalenderSystem.Application.IServices;
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

		public async Task SignOutAsync()
		{
			await _signInManager.SignOutAsync();
		}
	}
}
