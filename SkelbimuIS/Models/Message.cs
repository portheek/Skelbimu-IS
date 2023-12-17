namespace SkelbimuIS.Models
{    
    public class Message
    {
        public int id { get; set; }
        public string fromUsername{ get; set; }
        public string toUsername { get; set; }
        public string topic { get; set; }
        public string content { get; set; }
        public int reaction { get; set; }
        public DateTime date { get; set; }
    }
}