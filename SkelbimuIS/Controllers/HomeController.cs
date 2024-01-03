using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;

namespace SkelbimuIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataBaseModel database;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            database = new DataBaseModel(_httpContextAccessor);
        }

        public IActionResult Index()
        {
            List<Ad> ads = database.getAllAds();
            Console.WriteLine(ads.Capacity.ToString());
            return View(ads);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult DeleteAd(int AdId)
        {
            database.DeleteAd(AdId);
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}