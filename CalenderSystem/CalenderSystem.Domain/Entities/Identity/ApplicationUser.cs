using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
	{
		public string GoogleAccessToken { get; set; } = null!;
		public string GoogleRefreshToken { get; set; } = null!;
		public DateTime GoogleTokenExpiration { get; set; }
		// ---------- relations -----------
		public string GoogleCalenderId { get; set; } = null!;
		public ICollection<Event>? Events { get; set; }
	}
}
