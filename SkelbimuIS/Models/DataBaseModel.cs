using MySqlConnector;

namespace SkelbimuIS.Models
{
    public class DataBaseModel
    {
        private readonly string connectionString = "Server=localhost;Database=phpmyadmin;User ID=pma;Password=pmapass;Allow User Variables=true";
        private MySqlConnection connection;
        private PasswordHashService passwordHash;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DataBaseModel(IHttpContextAccessor httpContextAccessor)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            passwordHash = new PasswordHashService();
            _httpContextAccessor = httpContextAccessor;
        }

        //not used
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
            string sqlQuery = "SELECT * FROM users WHERE id=@id";
            
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);

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
        
        public List<Message> getCommonMessages(string currentUsername, string contactUsername)
        {

            string sqlQuery = "SELECT * FROM messages WHERE (fromUsername = @currentUsername AND toUsername = @contactUsername) OR (fromUsername = @contactUsername AND toUsername = @currentUsername) ORDER BY date;";
            List<Message> messages = new List<Message>();

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@currentUsername", currentUsername);
                command.Parameters.AddWithValue("@contactUsername", contactUsername);

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

        public List<string> getAllUserContacts(string username)
        {
                                                                       
            string sqlQuery = "SELECT DISTINCT personName FROM ( SELECT fromUsername AS personName FROM messages WHERE toUsername = @username UNION SELECT toUsername AS personName FROM messages WHERE fromUsername = @username) AS people_interacted;";
            List<string> userNames = new List<string>();

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {   
                command.Parameters.AddWithValue("@username", username);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read the person_id from the database
                        string usernameInteracted = reader["personName"].ToString();
                        
                        // Add the person_id to the list
                        userNames.Add(usernameInteracted);
                    }
                }
            }
            return userNames;  
        }

        public void addUser(User user)
        {
            string username = user.username;
            string password = passwordHash.HashPassword(user.password);
            string email = user.email;
            string role = user.role;

            string sqlQuery = "INSERT INTO users (username, email, password, role) VALUES (@username, @email, @password, @role);";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@role", role);

                command.ExecuteNonQuery();
            }
        }

        public bool userExists(string column, string value)
        {
            List<string> allowedColumns = new List<string> { "username", "email", "role", "id", "password"};
            if (!allowedColumns.Contains(column.ToLower()))
            {
                throw new ArgumentException("invalid column name.");
            }

            string sqlQuery = $"SELECT * FROM users WHERE {column} = @value";
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@value", value);

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

        public void addMessage(Message message)
        {
            string toUsername = message.toUsername;
            string fromUsername = message.fromUsername;
            string topic = message.topic;
            string content = message.content;
            int reaction = message.reaction;
            DateTime date = message.date;

            string sqlQuery = "INSERT INTO messages (fromUsername, toUsername, topic, message, reaction, date)" +
                  "VALUES (@fromUsername, @toUsername, @topic, @content, @reaction, @date);";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@fromUsername", fromUsername);
                command.Parameters.AddWithValue("@toUsername", toUsername);
                command.Parameters.AddWithValue("@topic", topic);
                command.Parameters.AddWithValue("@content", content);
                command.Parameters.AddWithValue("@reaction", reaction);
                command.Parameters.AddWithValue("@date", date);
                command.ExecuteNonQuery();
            }
        }

        public User getUser(string email, string password)
        {
            string hashedPassword = passwordHash.HashPassword(password);
            string sqlQuery = "SELECT * FROM users WHERE email = @email AND password = @hashedPassword";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@hashedPassword", hashedPassword);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {   
                            id = reader.GetInt32(0),
                            username = reader.GetString(1),
                            email = reader.GetString(2),
                            password = reader.GetString(3),
                            role = reader.GetString(4),
                        };

                        return user;
                    }
                }
            }
            return null;
        }
    }
}