using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;

namespace SkelbimuIS.Controllers
{
    public class AuthController : Controller
    {
        private DataBaseModel database = new DataBaseModel();
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string confirmpassword, string email)
        {
            Console.WriteLine("here");

            if (password != confirmpassword)
            {
                ViewBag.ErrorMessage = "Slaptažodžiai nesutampa!";
                return View();
            }

            if (database.userExists(username))
            {
                ViewBag.ErrorMessage = "Vartotojas su tokiu slapyvardžiu jau egzistuoja!";
                return View();
            }

            User user = new User
            {
                username = username,
                password = password,
                email = email,
                role = "user"
            };

            database.addUser(user);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
