using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Product> _products;

        public FavoritesController(IMongoClient client)
        {
            var db = client.GetDatabase("cantina");

            _users = db.GetCollection<User>("Users");
            _products = db.GetCollection<Product>("Products");
        }

        // ⭐ GET User Favorites
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavorites(string userId)
        {
            var user = await _users.Find(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            var favoriteProducts = await _products
    .Find(p => p.Id != null && user.Favorites.Contains(p.Id))
    .ToListAsync();

            return Ok(favoriteProducts);
        }

        // ⭐ ADD Favorite
        [HttpPost("add")]
        public async Task<IActionResult> AddFavorite(string userId, string productId)
        {
            var update = Builders<User>.Update.AddToSet(u => u.Favorites, productId);

            var result = await _users.UpdateOneAsync(
                u => u.Id == userId,
                update
            );

            if (result.ModifiedCount == 0)
                return BadRequest("Failed to add favorite.");

            return Ok(new { success = true, message = "Added to favorites." });
        }

        // ⭐ REMOVE Favorite
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFavorite(string userId, string productId)
        {
            var update = Builders<User>.Update.Pull(u => u.Favorites, productId);

            var result = await _users.UpdateOneAsync(
                u => u.Id == userId,
                update
            );

            if (result.ModifiedCount == 0)
                return BadRequest("Failed to remove favorite.");

            return Ok(new { success = true, message = "Removed from favorites." });
        }
    }
}
