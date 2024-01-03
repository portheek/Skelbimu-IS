using Microsoft.AspNetCore.Mvc;
using SkelbimuIS.Models;
using System.Text.Json;

namespace SkelbimuIS.Controllers
{
    public class SearchController : Controller
    {
        private readonly ILogger<SearchController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly User currentUser;
        private DataBaseModel database;
        public SearchController(ILogger<SearchController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            database = new DataBaseModel(_httpContextAccessor);
            var serializedUserObject = _httpContextAccessor.HttpContext.Session.GetString("UserObject");

            if (serializedUserObject == null)
            {
                currentUser = null;
            }
            else currentUser = JsonSerializer.Deserialize<User>(serializedUserObject);
        }



        public IActionResult Index(String query)
        {
            SearchModel model = new SearchModel();
            model.query = query;

            List<Ad> adList = database.getAllAds(currentUser, query);
            model.ads = adList;

            database.InsertSearchHistory(currentUser, model);

            return View(model);
        }

        public IActionResult AddAdToFavourites(int AdId, String query)
        {
            if (!database.CheckIfAdIsAddedToFavourites(currentUser, AdId))
            {
                database.AddAdToFavourites(currentUser, AdId);
            }

            return Redirect($"/Search/Index?query={query}");
        }

        public IActionResult RemoveAdFromFavourites(int AdId, string query)
        {
            if (database.CheckIfAdIsAddedToFavourites(currentUser, AdId))
            {
                database.RemoveAdFromFavourites(currentUser, AdId);
            }

            return Redirect($"/Search/Index?query={query}");
        }
    }
}
