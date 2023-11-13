using Microsoft.AspNetCore.Http;
using SkelbimuIS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SkelbimuIS.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public MessagesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewMessage()
        {
            return View();
        }

        public IActionResult NewMessage()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
