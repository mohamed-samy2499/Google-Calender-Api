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
		Task<SignInResult> ExternalLoginSignInAsync(ExternalLoginInfo info);
		Task SignOutAsync();
	}
}
