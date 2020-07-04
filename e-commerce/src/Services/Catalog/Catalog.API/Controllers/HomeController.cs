using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[Route("")]
	public class HomeController : Controller
	{
		[HttpGet()]
		// GET: /<controller>/
		public IActionResult Index()
		{
			return new RedirectResult("~/swagger");
		}
	}
}