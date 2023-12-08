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

        public MessagesController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Message> messages = ViewAllMessages();
            Console.WriteLine(messages[0].content);
            return View();
        }

        public List<Message> ViewAllMessages()
        {
            List<Message> messages = new List<Message>();

            using (MySqlConnection connection = DataBaseModel.GetConnection())
            {
                connection.Open();
                
                string sqlQuery = "SELECT * FROM messages";
                
                using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Message message = new Message();

                            message.id = reader.GetInt32(0);
                            message.fromId = reader.GetInt32(1);
                            message.toId = reader.GetInt32(2);
                            message.content = reader.GetString(3);
                            message.date = reader.GetDateTime(4);

                            messages.Add(message);
                        }
                    }
                }
            }
            return messages;
        }



        public IActionResult ViewMessage()
        {
            return View();
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

    public class Message
    {
        public int id { get; set; }
        public int fromId { get; set; }
        public int toId { get; set; }
        public string content { get; set; }
        public DateTime date { get; set; }
    }
}
