using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SkelbimuIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataBaseModel database;
        private readonly User currentUser;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            database = new DataBaseModel(_httpContextAccessor);

            var serializedUserObject = _httpContextAccessor.HttpContext.Session.GetString("UserObject");

            if (serializedUserObject == null)
            {
                currentUser = null;
            }
            else currentUser = JsonSerializer.Deserialize<User>(serializedUserObject);
        }

        public IActionResult Index()
        {
            if (currentUser == null)
            {
                ViewData["PleaseLogIn"] = "Norėdami matyti skelbimų sąrašą, prisijunkite.";
                return View(null);
            }

            List<Ad> ads = database.getAllAds(currentUser);
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

        public IActionResult AddAdToFavourites(int AdId)
        {
            if(!database.CheckIfAdIsAddedToFavourites(currentUser, AdId))
            {
                database.AddAdToFavourites(currentUser, AdId);
            }
            
            return Redirect("/Home/Index");
        }

        public IActionResult RemoveAdFromFavourites(int AdId)
        {
            if (database.CheckIfAdIsAddedToFavourites(currentUser, AdId))
            {
                database.RemoveAdFromFavourites(currentUser, AdId);
            }

            return Redirect("/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}