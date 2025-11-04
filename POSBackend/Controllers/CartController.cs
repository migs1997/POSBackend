using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;

namespace POSBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMongoCollection<CartItem> _cartCollection;
        private readonly IMongoCollection<Product> _productCollection;

        public CartController(IMongoClient client)
        {
            var database = client.GetDatabase("mobapp"); // Your DB name
            _cartCollection = database.GetCollection<CartItem>("cart");
            _productCollection = database.GetCollection<Product>("Products");
        }

        // Add item to cart
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.UserId) || string.IsNullOrEmpty(item.ProductId))
                return BadRequest(new { success = false, message = "userId and productId are required." });

            // Check if item already exists in cart
            var filter = Builders<CartItem>.Filter.Eq(c => c.UserId, item.UserId) &
                         Builders<CartItem>.Filter.Eq(c => c.ProductId, item.ProductId);

            var existing = await _cartCollection.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                await _cartCollection.ReplaceOneAsync(filter, existing);
            }
            else
            {
                item.Id = null; // MongoDB will generate Id
                await _cartCollection.InsertOneAsync(item);
            }

            return Ok(new { success = true, message = "Item added to cart" });
        }

        // Get cart items for a user
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            var cartItems = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();

            // Fetch product details
            var result = new List<object>();

            foreach (var cart in cartItems)
            {
                var product = await _productCollection.Find(p => p.Id == cart.ProductId).FirstOrDefaultAsync();
                if (product != null)
                {
                    result.Add(new
                    {
                        cart.Id,
                        cart.UserId,
                        cart.ProductId,
                        cart.Quantity,
                        ProdDesc = product.ProdDesc,
                        ImageUrl = product.ImageUrl,
                        ProdUnitPrice = product.ProdUnitPrice
                    });
                }
            }

            return Ok(new { success = true, cart = result });
        }

        // Remove item from cart
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] string userId, [FromQuery] string productId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest(new { success = false, message = "Invalid request" });

            var filter = Builders<CartItem>.Filter.Eq(c => c.UserId, userId) &
                         Builders<CartItem>.Filter.Eq(c => c.ProductId, productId);

            await _cartCollection.DeleteOneAsync(filter);

            return Ok(new { success = true, message = "Item removed from cart" });
        }
    }
}
