using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;
using System.Linq;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IMongoCollection<Favorite> _favCollection;

        public FavoritesController(IMongoClient client)
        {
            var database = client.GetDatabase("POSDatabase");
            _favCollection = database.GetCollection<Favorite>("favorites");
        }

        // Add product to favorites
        [HttpPost("add")]
        public IActionResult AddFavorite([FromBody] string productId, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest("UserId and ProductId are required.");

            var fav = _favCollection.Find(f => f.UserId == userId).FirstOrDefault();

            if (fav == null)
            {
                fav = new Favorite
                {
                    UserId = userId,
                    Products = new List<string> { productId }
                };
                _favCollection.InsertOne(fav);
            }
            else if (!fav.Products.Contains(productId))
            {
                fav.Products.Add(productId);
                _favCollection.ReplaceOne(f => f.Id == fav.Id, fav);
            }

            return Ok(fav);
        }

        // Get user favorites
        [HttpGet("{userId}")]
        public IActionResult GetFavorites(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required.");

            var fav = _favCollection.Find(f => f.UserId == userId).FirstOrDefault();
            return Ok(fav ?? new Favorite { UserId = userId });
        }

        // Remove product from favorites
        [HttpPost("remove")]
        public IActionResult RemoveFavorite([FromBody] string productId, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest("UserId and ProductId are required.");

            var fav = _favCollection.Find(f => f.UserId == userId).FirstOrDefault();
            if (fav != null)
            {
                fav.Products.Remove(productId);
                _favCollection.ReplaceOne(f => f.Id == fav.Id, fav);
            }

            return Ok(fav ?? new Favorite { UserId = userId });
        }
    }
}
