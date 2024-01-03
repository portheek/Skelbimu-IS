namespace SkelbimuIS.Models
{
    public class SearchModel
    {
        public String query { get; set; }
        public List<Ad> ads { get; set; }
        public String priceFrom { get; set; }
        public String priceTo { get; set; }
        public String city { get; set; }
        public String category { get; set; }
    }
}