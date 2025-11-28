using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;
using System.Collections.Generic;
using System.Linq;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IMongoCollection<Favorite> _favCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public FavoritesController(IMongoClient client)
        {
            var database = client.GetDatabase("POSDatabase");
            _favCollection = database.GetCollection<Favorite>("favorites");
            _productCollection = database.GetCollection<Product>("products");
        }

        // Add product to favorites
        [HttpPost("add")]
        public IActionResult AddFavorite([FromQuery] string userId, [FromBody] string productId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest(new { success = false, message = "UserId and ProductId are required." });

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

            return Ok(new { success = true, favorite = fav });
        }

        // Get user favorites (return full product data)
        [HttpGet("{userId}")]
        public IActionResult GetFavorites(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { success = false, message = "UserId is required." });

            var fav = _favCollection.Find(f => f.UserId == userId).FirstOrDefault();

            if (fav == null || fav.Products == null || fav.Products.Count == 0)
                return Ok(new { success = true, favorites = new List<Product>() });

            var productIds = fav.Products ?? new List<string>();

            var products = _productCollection
                .Find(p => p.Id != null && productIds.Contains(p.Id))   // ✔ No more warnings
                .ToList();

            return Ok(new { success = true, favorites = products });
        }


        // Remove a favorite product
        [HttpPost("remove")]
        public IActionResult RemoveFavorite([FromQuery] string userId, [FromBody] string productId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest(new { success = false, message = "UserId and ProductId are required." });

            var fav = _favCollection.Find(f => f.UserId == userId).FirstOrDefault();

            if (fav != null && fav.Products.Contains(productId))
            {
                fav.Products.Remove(productId);
                _favCollection.ReplaceOne(f => f.Id == fav.Id, fav);
            }

            return Ok(new { success = true });
        }
    }
}
