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
            List<Ad> ads = database.getAllAds(currentUser);
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
            List<Score> scores = database.getAllScores(AdId);
            Ad Model = database.getAdById(AdId);
            if (scores.Count > 0)
            {
                Model.ivertis = ScoreAverage(scores);
            }
            else
            {
                Model.ivertis = 0;
            }
            Model.reputacija = Reputation(Model.pardavejoId);
            return View("ViewAd", Model);
        }

        [HttpPost]
        public IActionResult AddScore(int AdId, decimal score)
        {
            List<Score> scores = database.getAllScores(AdId);
            Ad Model = database.getAdById(AdId);
            if (currentUser == null)
            {
                ViewBag.ErrorMessage = "Esate neprisijungias!";
                return View("ViewAd", Model);
            }
            if (ScoreUser(scores))
            {
                ViewBag.ErrorMessage = "Jau esate įvertines šį skelbimą!";
                return View("ViewAd", Model);
            }
            if (currentUser.id == Model.pardavejoId)
            {
                ViewBag.ErrorMessage = "Negalima vertinti savo skelbimo!";
                return View("ViewAd", Model);
            }
            Score s = new Score
            {
                pardavejoId = currentUser.id,
                skelbimoId = AdId,
                ivertis = score,
                data = DateTime.Now
            };
            scores.Add(s);
            database.addScore(s);
            ViewBag.SuccessMessage = "Ačiū už jusų įvertinimą!";
            Model.ivertis = ScoreAverage(scores);
            database.UpdateAd(Model);
            return ViewAd(AdId);
        }

        public decimal Reputation(int pardId)
        {
            decimal avg = 0;
            List<Ad> ads = database.getAllSellersAds(pardId);
            if (ads.Any())
            {
                foreach (Ad ad in ads)
                {
                    List<Score> scores = database.getAllScores(ad.id);
                    avg = avg + ScoreAverage(scores);
                }
                avg = avg / ads.Count;
            }
            return avg;
        }

        public bool ScoreUser(List<Score> scores)
        {

            foreach (Score score in scores)
            {
                if (score.pardavejoId == currentUser.id)
                {
                    return true;
                }
            }
            return false;
        }

        public decimal ScoreAverage(List<Score> scores)
        {
            decimal avg = 0;
            if (scores.Count > 0)
            {
                foreach (Score score in scores)
                {
                    avg = avg + score.ivertis;
                }
                avg = avg / scores.Count;
            }
            return avg;
        }

        public IActionResult EditAd(int id)
        {
            Ad Model = database.getAdById(id);
            return View("EditAd", Model);
        }

        [HttpPost]
        public IActionResult UpdateAd(string title, string description, string phone, string category, string city, int price, int Adid)
        {
            Ad Model = database.getAdById(Adid);
            if (title == "" || description == "" || phone == "" || category == "" || city == "" || price == 0)
            {
                ViewBag.ErrorMessage = "Užpildti ne visi laukai!";
                return View("EditAd", Model);
            }
            if (!Regex.IsMatch(phone, @"^[0-9\+]+$"))
            {
                ViewBag.ErrorMessage = "Numeris turėtų būt sudaromas iš skaičių ir pliuso!";
                return View("EditAd", Model);
            }
            if (price <= 0)
            {
                ViewBag.ErrorMessage = "Kaina negali tapti neigiama ar nuliu!";
                return View("EditAd", Model);
            }
            Ad ad = new Ad
            {
                id = Adid,
                pavadinimas = title,
                numeris = phone,
                aprasas = description,
                kaina = price,
                ivertis = ScoreAverage(database.getAllScores(Adid)),
                reputacija = 0,
                miestas = city,
                perziuros = 0,
                data = DateTime.Now,
                megst = false,
                kategorija = category
            };

            database.UpdateAd(ad);
            return ViewAd(Adid);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Ad> ads = database.getAllAds(currentUser);
            return View(ads);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}