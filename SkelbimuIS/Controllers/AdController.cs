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
        public AdController(ILogger<AdController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;

            var serializedUserObject = _httpContextAccessor.HttpContext.Session.GetString("UserObject");
            
            if (serializedUserObject == null){
                currentUser = null;
            }
            else currentUser = JsonSerializer.Deserialize<User>(serializedUserObject);
        }

        public IActionResult Index()
        {
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}