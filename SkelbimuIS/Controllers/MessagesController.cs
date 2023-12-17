using SkelbimuIS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Web;


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
            var model = new Tuple<List<Message>, User>(messages, currentUser);
            return View("ViewMessages", model);
        }

        public IActionResult NewMessage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string toUsername, string topic, string message)
        {
            if(toUsername == "" || topic == "" || message == "")
            {
                ViewBag.ErrorMessage = "Užpildti ne visi laukai!";
                return View("NewMessage");
            }

            if(!database.userExists("username", toUsername) && toUsername != "")
            {
                ViewBag.ErrorMessage = "Toks vartotojas neegzistuoja!";
                return View("NewMessage");
            }
            
            if(currentUser.username == toUsername)
            {
                ViewBag.ErrorMessage = "Negalima siųsti žinutės sau!";
                return View("NewMessage");
            }
            
            Message newMessage = new Message()
            {
                fromUsername = currentUser.username,
                toUsername = toUsername,
                topic = topic,
                content = message,
                reaction = 0,
                date = DateTime.Now
            };
            database.addMessage(newMessage);
            
            ViewBag.SuccessMessage = "Žinutė išsiųsta!";
            return ViewMessages(toUsername);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

   
}
