using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SkelbimuIS.Controllers
{
    public class AdController : Controller
    {
        private readonly ILogger<AdController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly User currentUser;
        private DataBaseModel database;
        public AdController(ILogger<AdController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            database = new DataBaseModel(_httpContextAccessor);
            var serializedUserObject = _httpContextAccessor.HttpContext.Session.GetString("UserObject");
            
            if (serializedUserObject == null){
                currentUser = null;
            }
            else currentUser = JsonSerializer.Deserialize<User>(serializedUserObject);
        }

        public IActionResult Index()
        {
            List<Ad> ads = database.getAllAds();
            return View();
        }

        public IActionResult CreateAd()
        {
            if (currentUser == null){
                ViewBag.ErrorMessage = "Jūs neprisijungęs!";
                return View();
            }
            
            return View();
        }

        public IActionResult ViewAd()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ad> ads = database.getAllAds();
            Console.WriteLine("reeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
            return View(ads);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}