using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using SkelbimuIS.Models;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

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

            if (serializedUserObject == null)
            {
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
            if (currentUser == null)
            {
                ViewBag.ErrorMessage = "Jūs neprisijungęs!";
                return View();
            }

            return View();
        }

        [HttpPost]
        public IActionResult CreateAdMethod(string title, string description, string phone, string category, string city, int price)
        {
            if (title == "" || description == "" || phone == "" || category == "" || city == "" || price == 0)
            {
                ViewBag.ErrorMessage = "Užpildti ne visi laukai!";
                return View("CreateAd");
            }
            if (!Regex.IsMatch(phone, @"^[0-9\+]+$"))
            {
                ViewBag.ErrorMessage = "Numeris turėtų būt sudaromas iš skaičių ir pliuso!";
                return View("CreateAd");
            }
            if (price <= 0)
            {
                ViewBag.ErrorMessage = "Kaina negali tapti neigiama ar nuliu!";
                return View("CreateAd");
            }
            Ad ad = new Ad
            {
                pavadinimas = title,
                numeris = phone,
                pastas = currentUser.email,
                aprasas = description,
                kaina = price,
                ivertis = 0,
                reputacija = 0,
                miestas = city,
                perziuros = 0,
                data = DateTime.Now,
                megst = false,
                pardavejoId = currentUser.id,
                kategorija = category
            };

            int id = database.addAd(ad);
            return ViewAd(id);
        }

        public IActionResult ViewAd(int AdId)
        {
            Ad Model = database.getAdById(AdId);
            return View("ViewAd", Model);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ad> ads = database.getAllAds();
            return View(ads);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}