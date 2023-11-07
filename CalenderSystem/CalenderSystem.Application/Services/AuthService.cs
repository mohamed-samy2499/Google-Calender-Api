using CalenderSystem.Application.Helper;
using CalenderSystem.Application.IServices;
using CalenderSystem.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using CalenderSystem.Application.DTOs;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using MovieSystem.Application.Services;
using CalenderSystem.Infrastructure.Repositories.ApplicationUserRepositories;

namespace CalenderSystem.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;

        private readonly IApplicationUserRepository _applicationUserRepository;

        public AuthService(SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager,
            IApplicationUserRepository applicationUserRepository)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_httpClient = new HttpClient();
            _applicationUserRepository = applicationUserRepository;
        }
		//getting the user email and google Id
        public async Task<UserInfoResponseDTO> GetUserInfoAsync(string accessToken)
        {
            var userInfo = new UserInfoResponseDTO();
            var userInfoResponse = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken);
            if (userInfoResponse.IsSuccessStatusCode)
            {
                var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
                userInfo = JsonConvert.DeserializeObject<UserInfoResponseDTO>(userInfoContent);
            }
            return userInfo;
        }
		//adding or updating a user after signing in using google
        public async Task<bool> AddOrUpdateUserAsync(GoogleTokenResponseDTO tokens,
			UserInfoResponseDTO userInfo)
        {
			try
			{

				var googleUserId = userInfo.Id;
				var email = userInfo.email;
				var user = await _applicationUserRepository.GetByEmailAsync(email);
				//var user = await _userManager.FindByEmailAsync(email);
				DateTime currentDateTime = DateTime.Now;
				DateTime expirationDateTime = currentDateTime.AddSeconds(tokens.Expires_in);
				if (user == null)
				{
					user = new ApplicationUser
					{
						UserName = email,
						Email = email,
						GoogleUserId = googleUserId,
						GoogleAccessToken = tokens.Access_token,
						GoogleRefreshToken = tokens.Refresh_token,
						GoogleTokenExpiration = expirationDateTime
					};
					var result = await _applicationUserRepository.CreateAsync(user);
					if (result == null)
					{
						return false;
					}
					await _signInManager.SignInAsync(user, isPersistent: false);
					return true;
				}
				else
				{
					user.GoogleAccessToken = tokens.Access_token;
					user.GoogleRefreshToken = tokens.Refresh_token;
                
					user.GoogleTokenExpiration = expirationDateTime;
					// Update the user in the database
					var updateResult = await _applicationUserRepository.UpdateAsync(user);
					if (updateResult == null)
						return false;
					return true;
				}
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
				return false;
			}
        }

   
		//a method for generating the google sign in link 
		public string GetAuthCode(string authUrl, string redirectUrl, string clientId)
		{
			string scopeURL1 = authUrl + "?redirect_uri={0}&prompt={1}&response_type={2}&client_id={3}&scope={4}&access_type={5}";

			var redirectURL = redirectUrl;

			string prompt = "consent";

			string response_type = "code";
			string clientID = clientId;
			string scope = "https://www.googleapis.com/auth/calendar + https://www.googleapis.com/auth/calendar.events  + https://www.googleapis.com/auth/userinfo.email + https://www.googleapis.com/auth/userinfo.profile";

            string access_type = "offline";
			string redirect_uri_encode = redirectURL;//Method.urlEncodeForGoogle(redirectURL);
			var mainURL = string.Format(scopeURL1, redirect_uri_encode, prompt, response_type, clientID, scope, access_type);

			return mainURL;
		}
		//method for getting the token after the user successfully sign in using google
		public async Task<GoogleTokenResponseDTO> GetTokens(string code, string redirectUrl,
			string clientId, string clientSecret, string tokenUrl)
		{

			var content = new StringContent($"code={code}&redirect_uri={Uri.EscapeDataString(redirectUrl)}&client_id={clientId}&client_secret={clientSecret}&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");

			var response = await _httpClient.PostAsync(tokenUrl, content);
			var responseContent = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode)
			{
				var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponseDTO>(responseContent);
				return tokenResponse;
			}
			else
			{
				// Handle the error case when authentication fails
				throw new Exception($"Failed to authenticate: {responseContent}");
			}
		}


        public async Task SignOutAsync()
		{
			await _signInManager.SignOutAsync();
		}
	}
}
