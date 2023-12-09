using Microsoft.AspNetCore.Http;
using SkelbimuIS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySqlConnector;


namespace SkelbimuIS.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DataBaseModel database = new DataBaseModel();


        public MessagesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Message> messages = database.getAllMessages();
            return View(messages);
        }

        public IActionResult ViewMessage(string username)
        {
            List<Message> messages = database.getAllUserMessages(username);
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
