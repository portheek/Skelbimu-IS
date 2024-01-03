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

        public List<Ad> getAllAds(User user, String searchQuery = null, String priceFrom = null, String priceTo = null, String city = null, String category = null)
        {
            List<Ad> ads = new List<Ad>();

            string sqlQuery = "";

            //sqlQuery = "SELECT * FROM ad WHERE pavadinimas LIKE '%{0}%' AND kaina > priceFrom AND kaina < priceTo AND miestas = city AND kategorija = category";

            if (searchQuery != null)
            {
                sqlQuery = String.Format("SELECT * FROM ad WHERE pavadinimas LIKE '%{0}%' AND", searchQuery);
            }
            else
            {
                sqlQuery = "SELECT * FROM ad WHERE ";
            }

            int priceFromInt = 0, priceToInt = int.MaxValue;

            if (priceFrom != null)
            {
                int.TryParse(priceFrom, out priceFromInt);
            }
                

            if (priceTo != null)
            {
                int.TryParse(priceTo, out priceToInt);
            }
                


            sqlQuery = String.Format($"{sqlQuery} kaina > {priceFromInt} AND kaina < {priceToInt}");

            if(city != null)
            {
                sqlQuery = String.Format($"{sqlQuery} AND miestas = \"{city}\"");
            }

            if (category != null)
            {
                sqlQuery = String.Format($"{sqlQuery} AND kategorija = \"{category}\"");
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

            foreach(Ad ad in ads)
            {
                bool isFavourite = CheckIfAdIsAddedToFavourites(user, ad.id);
                ad.megst = isFavourite;
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

        internal void DeleteAd(int adId)
        {
            string sqlQuery = "DELETE FROM ad WHERE id=@id";
            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@id", adId);
                command.ExecuteNonQuery();
            }
        }

        internal List<Score> getAllScores(int adId)
        {
            List<Score> scores = new List<Score>();

            string sqlQuery = "SELECT * FROM score WHERE adid=@id";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@id", adId);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Score score = new Score
                        {
                            pardavejoId = reader.GetInt32(0),
                            skelbimoId = reader.GetInt32(1),
                            ivertis = reader.GetDecimal(2),
                            data = reader.GetDateTime(3),
                        };
                        scores.Add(score);
                    }
                }
            }
            return scores;
        }

        public void addScore(Score score)
        {
            int userId = score.pardavejoId;
            int adId = score.skelbimoId;
            decimal ivertis = score.ivertis;


            string sqlQuery = $"INSERT INTO score (userid, adid, ivertis) " +
                $"VALUES ('{userId}', '{adId}', '{ivertis}');";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("userid", userId);
                command.Parameters.AddWithValue("adid", adId);
                command.Parameters.AddWithValue("ivertis", ivertis);

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

            string sqlQuery = "INSERT INTO search_history (user, query, category, price_from, price_to, city) " +
                  "VALUES (@userid, @query, @category, @priceFrom, @priceTo, @city);";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@query", query);
                command.Parameters.AddWithValue("@category", model.category);
                command.Parameters.AddWithValue("@priceFrom", model.priceFrom);
                command.Parameters.AddWithValue("@priceTo", model.priceTo);
                command.Parameters.AddWithValue("@city", model.city);
                command.ExecuteNonQuery();
            }
        }

        public List<SearchModel> GetUserSearchHistory(User user)
        {
            if (user == null)
                return null;

            List<SearchModel> queries = new List<SearchModel>();
            int userid = user.id;
            Console.WriteLine($"User id: {userid};");
            string sqlQuery = "SELECT id, query, price_from, price_to, city, category FROM search_history WHERE user = @userid ORDER BY id DESC LIMIT 10 ";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SearchModel search = new SearchModel();
                        search.query = reader["query"].ToString();
                        search.priceFrom = reader["price_from"].ToString();
                        search.priceTo = reader["price_to"].ToString();
                        search.city = reader["city"].ToString();
                        search.category = reader["category"].ToString();
                        queries.Add(search);
                    }
                }
            }
            return queries;
        }

        public bool CheckIfAdIsAddedToFavourites(User user, int id)
        {
            if(user == null)
            {
                return false;
            }

            int userid = user.id;


            string sqlQuery = "SELECT id FROM favourite_ads WHERE user = @userid AND ad = @adid";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@adid", id);
                command.ExecuteNonQuery();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddAdToFavourites(User user, int id)
        {
            int userid = user.id;


            string sqlQuery = "INSERT INTO favourite_ads (user, ad) " +
                  "VALUES (@userid, @adid);";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@adid", id);
                command.ExecuteNonQuery();
            }
        }

        public void RemoveAdFromFavourites(User user, int id)
        {
            int userid = user.id;


            string sqlQuery = "DELETE FROM favourite_ads WHERE user = @userid AND ad = @adid";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@adid", id);
                command.ExecuteNonQuery();
            }
        }

        public String GetMostSearchedCategory(User user)
        {
            if (user == null)
                return null;

            int userid = user.id;
            string sqlQuery = $"SELECT category, COUNT(category) AS `value_occurrence` FROM search_history WHERE user = {userid} GROUP BY category ORDER BY `value_occurrence` DESC LIMIT 1";

            using (MySqlCommand command = new MySqlCommand(sqlQuery, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
            }
            return null;
        }

        public List<Ad> getRecommendedAds(User user)
        {
            List<Ad> ads = new List<Ad>();

            if (user == null)
                return ads;

            string sqlQuery = "";

            if (GetUserSearchHistory(user).Count < 1)
            {
                sqlQuery = $"SELECT * FROM ad ORDER BY reputacija, ivertis DESC LIMIT 3";
            }
            else
            {
                string category = GetMostSearchedCategory(user);
                sqlQuery = $"SELECT * FROM ad WHERE kategorija = \"{category}\" ORDER BY reputacija, ivertis DESC LIMIT 3";
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

                        Console.WriteLine("Yes");

                        ads.Add(ad);
                    }
                }
            }

            foreach (Ad ad in ads)
            {
                bool isFavourite = CheckIfAdIsAddedToFavourites(user, ad.id);
                ad.megst = isFavourite;
            }
            return ads;
        }

    }
}