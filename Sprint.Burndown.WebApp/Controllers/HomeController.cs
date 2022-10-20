using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private IBoardService BoardService { get; }

        public HomeController(IBoardService boardService)
        {
            BoardService = boardService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(model);
        }
    }
}
