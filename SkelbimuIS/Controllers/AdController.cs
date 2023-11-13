using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;

namespace SkelbimuIS.Controllers
{
    public class AdController : Controller
    {
        private readonly ILogger<AdController> _logger;

        public AdController(ILogger<AdController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateAd()
        {
            return View();
        }

        public IActionResult ViewAd()
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