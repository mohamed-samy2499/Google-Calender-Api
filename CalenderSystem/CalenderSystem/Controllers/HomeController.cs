using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalenderSystem.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Home Index");
		}
	}
}
