using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sprint.Burndown.WebApp.Controllers
{
    [Authorize]
    public class EstimatesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}