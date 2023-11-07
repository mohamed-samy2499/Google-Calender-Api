using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalenderSystem.Application.DTOs
{
	public class GoogleTokenResponseDTO
	{
		public string Access_token
		{
			get;
			set;
		} = null!;

		public long Expires_in
		{
			get;
			set;
		}

		public string Refresh_token
		{
			get;
			set;
		} = null!;

		public string Scope
		{
			get;
			set;
		} = null!;

		public string Token_type
		{
			get;
			set;
		} = null!;
	}
}
