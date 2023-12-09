using System;
using MySqlConnector;

namespace SkelbimuIS.Models
{
    public class DataBaseModel
    {
        string connectionString = "Server=localhost;Database=phpmyadmin;User ID=pma;Password=pmapass;";
        private MySqlConnection connection;

        public DataBaseModel()
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public List<Message> getAllMessages()
        {
            List<Message> messages = new List<Message>();

            string sqlQuery = "SELECT * FROM messages";
            
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Message message = new Message();

                        message.id = reader.GetInt32(0);
                        message.fromUsername = reader.GetString(1);
                        message.toUsername = reader.GetString(2);
                        message.topic = reader.GetString(3);
                        message.content = reader.GetString(4);
                        message.reaction = reader.GetInt32(5);
                        message.date = reader.GetDateTime(6);

                        messages.Add(message);
                    }
                }
            }
            return messages;
        }

        public User getUserById(int id){
            
            string sqlQuery = $"SELECT * FROM users WHERE id={id}";
            
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User();

                        user.id = reader.GetInt32(0);
                        user.username = reader.GetString(1);
                        user.email = reader.GetString(2);
                        user.role = reader.GetString(4);

                        return user;
                    }
                }
            }
            return null;
        }
        
        public List<Message> getAllUserMessages(string username){
                                                            // AND ReceiverID = @User2                    //AND ReceiverID = @User1
            string sqlQuery = $"SELECT * FROM messages WHERE (fromUsername = '{username}') OR (toUsername = '{username}') ORDER BY date;";
            List<Message> messages = new List<Message>();

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Message message = new Message();

                        message.id = reader.GetInt32(0);
                        message.fromUsername = reader.GetString(1);
                        message.toUsername = reader.GetString(2);
                        message.topic = reader.GetString(3);
                        message.content = reader.GetString(4);
                        message.reaction = reader.GetInt32(5);
                        message.date = reader.GetDateTime(6);

                        messages.Add(message);
                    }
                }
            }
            return messages;  
        }
    }
}