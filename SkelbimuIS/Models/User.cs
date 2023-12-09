using Microsoft.AspNetCore.Http;
using SkelbimuIS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MySqlConnector;

namespace SkelbimuIS.Models
{    
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}