namespace SkelbimuIS.Models
{
    public class Ad
    {
        public int id { get; set; }
        public string pavadinimas { get; set; }
        public string numeris { get; set; }
        public string pastas { get; set; }
        public string aprasas { get; set; }
        public decimal kaina { get; set; }
        public decimal ivertis { get; set; }
        public bool megst { get; set; }
        public decimal reputacija { get; set; }
        public string miestas { get; set; }
        public int perziuros { get; set; }
        public DateTime data { get; set; }
        public int pardavejoId { get; set; }
        public string kategorija { get; set; }
    }
}
