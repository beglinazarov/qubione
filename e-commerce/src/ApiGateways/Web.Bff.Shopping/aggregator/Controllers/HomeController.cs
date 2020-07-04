
using Microsoft.AspNetCore.Mvc;

namespace e_commerce.ApiGateways.Web.Bff.Shopping.HttpAggregator
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet()]
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}