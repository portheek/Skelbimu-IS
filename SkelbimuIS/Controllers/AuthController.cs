using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SkelbimuIS.Controllers
{
    public class AuthController : Controller
    {
        private DataBaseModel database;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            database = new DataBaseModel(_httpContextAccessor);

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {

            User user = database.getUser(email, password);
            if (user == null)
            {
                ViewBag.ErrorMessage = "Tokio vartotojo nėra!";
                return View("Index");
            }

            SetSessionUser(user);
            ViewBag.SuccessMessage = "Prisijungta!";
            return View("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string confirmpassword, string email)
        {

            if (username == "" ||  password == "" || confirmpassword == "" || email == ""){
                ViewBag.ErrorMessage = "Forma nebaigta pildyti!";
                return View();
            }

            if (password != confirmpassword)
            {
                ViewBag.ErrorMessage = "Slaptažodžiai nesutampa!";
                return View();
            }

            if (database.userExists("username", username))
            {
                ViewBag.ErrorMessage = "Vartotojas su tokiu slapyvardžiu jau egzistuoja!";
                return View();
            }

            if (database.userExists("email", email))
            {
                ViewBag.ErrorMessage = "Vartotojas su tokiu el. paštu jau egzistuoja!";
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
        
        //role, username
        public IActionResult SetSessionUser(User user)
        {

            // Serialize and store the object in session
            var serializedUserObject = JsonSerializer.Serialize(user);
            _httpContextAccessor.HttpContext.Session.SetString("UserObject", serializedUserObject);

            return new OkResult();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
