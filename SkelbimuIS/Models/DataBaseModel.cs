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
                        Message message = new Message
                        {
                            id = reader.GetInt32(0),
                            fromUsername = reader.GetString(1),
                            toUsername = reader.GetString(2),
                            topic = reader.GetString(3),
                            content = reader.GetString(4),
                            reaction = reader.GetInt32(5),
                            date = reader.GetDateTime(6)
                        };

                        messages.Add(message);
                    }
                }
            }
            return messages;
        }

        public User getUserById(int id)
        {
            
            string sqlQuery = $"SELECT * FROM users WHERE id={id}";
            
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            id = reader.GetInt32(0),
                            username = reader.GetString(1),
                            email = reader.GetString(2),
                            role = reader.GetString(4)
                        };

                        return user;
                    }
                }
            }
            return null;
        }
        
        public List<Message> getAllUserMessages(string username)
        {
                                                            // AND ReceiverID = @User2                    //AND ReceiverID = @User1
            string sqlQuery = $"SELECT * FROM messages WHERE (fromUsername = '{username}') OR (toUsername = '{username}') ORDER BY date;";
            List<Message> messages = new List<Message>();

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Message message = new Message
                        {
                            id = reader.GetInt32(0),
                            fromUsername = reader.GetString(1),
                            toUsername = reader.GetString(2),
                            topic = reader.GetString(3),
                            content = reader.GetString(4),
                            reaction = reader.GetInt32(5),
                            date = reader.GetDateTime(6)
                        };

                        messages.Add(message);
                    }
                }
            }
            return messages;  
        }

        public void addUser(User user)
        {
            string username = user.username;
            string password = user.HashPassword();
            string email = user.email;
            string role = user.role;

            Console.WriteLine(username);

            string sqlQuery = $"INSERT INTO users (username, email, password, role) VALUES ('{username}', '{email}', '{password}', '{role}');";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("username", username);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("password", password);
                command.Parameters.AddWithValue("role", role);

                command.ExecuteNonQuery();
            }
        }

        public bool userExists(string username){

            string sqlQuery = $"SELECT * FROM users WHERE username = '{username}'";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("username", username);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }  
                    return false;
                }
            }
        }
    }
}