using SkelbimuIS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;


namespace SkelbimuIS.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DataBaseModel database;
        private readonly User currentUser;

        public MessagesController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
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
            
            if (currentUser == null){
                ViewBag.ErrorMessage = "Jūs neprisijungęs!";
                return View(null);
            }
            List<string> messages = database.getAllUserContacts(currentUser.username);
            return View(messages);
        }

        public IActionResult ViewMessages(string contactUsername)
        {
            List<Message> messages = database.getCommonMessages(currentUser.username, contactUsername);
            return View(messages);
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
