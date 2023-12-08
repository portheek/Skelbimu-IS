using System;
using MySqlConnector;

namespace SkelbimuIS.Models
{
    public class DataBaseModel
    {
        public static MySqlConnection GetConnection(){
            string connectionString = "Server=localhost;Database=phpmyadmin;User ID=pma;Password=pmapass;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }
    }
}