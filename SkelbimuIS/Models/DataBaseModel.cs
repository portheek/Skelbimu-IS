using MySqlConnector;

namespace SkelbimuIS.Models
{
    public class DataBaseModel
    {
        private readonly string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=skelbimai;Allow User Variables=true;";// "Server=localhost;Database=phpmyadmin;User ID=pma;Password=pmapass;Allow User Variables=true";
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

        public List<Ad> getAllAds(String searchQuery = null)
        {
            List<Ad> ads = new List<Ad>();

            string sqlQuery = "SELECT * FROM ad";

            if (searchQuery != null)
            {
                sqlQuery = String.Format("SELECT * FROM ad WHERE pavadinimas LIKE '%{0}%'", searchQuery);
            }
   
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ad ad = new Ad
                        {
                            id = reader.GetInt32(0),
                            pavadinimas = reader.GetString(1),
                            numeris = reader.GetString(2),
                            pastas = reader.GetString(3),
                            aprasas = reader.GetString(4),
                            kaina = reader.GetDecimal(5),
                            ivertis = reader.GetDecimal(6),
                            reputacija = reader.GetDecimal(7),
                            miestas = reader.GetString(8),
                            perziuros = reader.GetInt32(9),
                            data = reader.GetDateTime(10),
                            megst = reader.GetBoolean(11),
                            pardavejoId = reader.GetInt32(12),
                            kategorija = reader.GetString(13)
                        };
                        ads.Add(ad);
                    }
                }
            }
            return ads;
        }

        internal Ad getAdById(int adId)
        {
            string sqlQuery = "SELECT * FROM ad WHERE id=@adId";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@adId", adId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Ad ad = new Ad
                        {
                            id = reader.GetInt32(0),
                            pavadinimas = reader.GetString(1),
                            numeris = reader.GetString(2),
                            pastas = reader.GetString(3),
                            aprasas = reader.GetString(4),
                            kaina = reader.GetDecimal(5),
                            ivertis = reader.GetDecimal(6),
                            reputacija = reader.GetDecimal(7),
                            miestas = reader.GetString(8),
                            perziuros = reader.GetInt32(9),
                            data = reader.GetDateTime(10),
                            megst = reader.GetBoolean(11),
                            pardavejoId = reader.GetInt32(12),
                            kategorija = reader.GetString(13)
                        };
                        return ad;
                    }
                }
            }
            return null;
        }

        public int addAd(Ad ad)
        {
            string pavadinimas = ad.pavadinimas;
            string numeris = ad.numeris;
            string pastas = ad.pastas;
            string aprasas = ad.aprasas;
            decimal kaina = ad.kaina;
            decimal ivertis = ad.ivertis;
            decimal reputacija = ad.reputacija;
            string miestas = ad.miestas;
            int perziuros = ad.perziuros;
            DateTime data = ad.data;
            bool megst = ad.megst;
            int pardId = ad.pardavejoId;
            string kategorija = ad.kategorija;

            Console.WriteLine(pavadinimas);

            string sqlQuery = $"INSERT INTO ad (pavadinimas, numeris, pastas, aprasas, kaina, ivertis, reputacija, miestas, perziuros, megst, pardId, kategorija) " +
                $"VALUES ('{pavadinimas}', '{numeris}', '{pastas}', '{aprasas}', '{kaina}', '{ivertis}', '{reputacija}', '{miestas}', '{perziuros}', '{megst}', '{pardId}', '{kategorija}');";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("pavadinimas", pavadinimas);
                command.Parameters.AddWithValue("numeris", numeris);
                command.Parameters.AddWithValue("pastas", pastas);
                command.Parameters.AddWithValue("aprasas", aprasas);
                command.Parameters.AddWithValue("kaina", kaina);
                command.Parameters.AddWithValue("ivertis", ivertis);
                command.Parameters.AddWithValue("reputacija", reputacija);
                command.Parameters.AddWithValue("miestas", miestas);
                command.Parameters.AddWithValue("perziuros", perziuros);
                command.Parameters.AddWithValue("megst", megst);
                command.Parameters.AddWithValue("pardId", pardId);
                command.Parameters.AddWithValue("kategorija", kategorija);

                command.ExecuteNonQuery();

                command.CommandText = "SELECT LAST_INSERT_ID();";
                int lastInsertedId = Convert.ToInt32(command.ExecuteScalar());

                return lastInsertedId;
            }
        }

        public void UpdateAd(Ad ad)
        {
            int id = ad.id;
            string pavadinimas = ad.pavadinimas;
            string numeris = ad.numeris;
            string aprasas = ad.aprasas;
            decimal kaina = ad.kaina;
            string miestas = ad.miestas;
            string kategorija = ad.kategorija;

            string sqlQuery = @"UPDATE ad
                        SET pavadinimas = @pavadinimas, 
                            numeris = @numeris,  
                            aprasas = @aprasas, 
                            kaina = @kaina, 
                            miestas = @miestas,   
                            kategorija = @kategorija
                        WHERE id = @id;";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@pavadinimas", pavadinimas);
                command.Parameters.AddWithValue("@numeris", numeris);
                command.Parameters.AddWithValue("@aprasas", aprasas);
                command.Parameters.AddWithValue("@kaina", kaina);
                command.Parameters.AddWithValue("@miestas", miestas);
                command.Parameters.AddWithValue("@kategorija", kategorija);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
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

        public void updateMessageReaction(int score, int messageId)
        {   
            string sqlQuery = "UPDATE messages SET reaction=reaction+@score WHERE id=@id";
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {   
                command.Parameters.AddWithValue("@id", messageId);
                command.Parameters.AddWithValue("@score", score);
                command.ExecuteNonQuery();
            }
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

        public void deleteMessage(int id)
        {
            string sqlQuery = "DELETE FROM messages WHERE id=@id";
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@id", id);
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

        public void InsertSearchHistory(User user, SearchModel model)
        {
            int userid = user.id;
            String query = model.query;

            if(query == null)
            {
                return;
            }

            string sqlQuery = "INSERT INTO search_history (user, query) " +
                  "VALUES (@userid, @query);";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@query", query);
                command.ExecuteNonQuery();
            }
        }

        public List<String> GetUserSearchHistory(User user)
        {
            List<String> queries = new List<String>();
            int userid = user.id;
            Console.WriteLine($"User id: {userid};");
            string sqlQuery = "SELECT query FROM search_history WHERE user = @userid LIMIT 10";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        String query = reader["query"].ToString();
                        Console.WriteLine($"got query: {query};");
                        queries.Add(query);
                    }
                }
            }
            return queries;
        }
    }
}