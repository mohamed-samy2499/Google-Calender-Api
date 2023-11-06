using CalenderSystem.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.IServices
{
	public interface IAuthService
	{
		string GetAuthCode(string authUrl, string redirectUrl, string clientId);
		Task<GoogleTokenResponseDTO> GetTokens(string code, string redirectUrl,
			string clientId, string clientSecret, string tokenUrl);
		Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
		Task SignOutAsync();
	}
}
