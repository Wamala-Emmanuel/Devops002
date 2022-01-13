using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers
{
    [AllowAnonymous]
    public class DocumentationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("api-docs")]
        public IActionResult Docs()
        {
            return View();
        }
    }
}