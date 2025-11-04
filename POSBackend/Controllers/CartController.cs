using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;
using System.Linq;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMongoCollection<CartItem> _cartCollection;

        public CartController(IMongoClient client)
        {
            var database = client.GetDatabase("POSDatabase");
            _cartCollection = database.GetCollection<CartItem>("carts");
        }

        // Add item to cart
        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] Item item, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(item.ProductId))
                return BadRequest("UserId and ProductId are required.");

            var cart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();

            if (cart == null)
            {
                cart = new CartItem
                {
                    UserId = userId,
                    Items = new List<Item> { item }
                };
                _cartCollection.InsertOne(cart);
            }
            else
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                    existingItem.Quantity += item.Quantity;
                else
                    cart.Items.Add(item);

                _cartCollection.ReplaceOne(c => c.Id == cart.Id, cart);
            }

            return Ok(cart);
        }

        // Get user cart
        [HttpGet("{userId}")]
        public IActionResult GetCart(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required.");

            var cart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();
            return Ok(cart ?? new CartItem { UserId = userId });
        }

        // Remove item from cart
        [HttpPost("remove")]
        public IActionResult RemoveFromCart([FromBody] string productId, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
                return BadRequest("UserId and ProductId are required.");

            var cart = _cartCollection.Find(c => c.UserId == userId).FirstOrDefault();
            if (cart != null)
            {
                cart.Items.RemoveAll(i => i.ProductId == productId);
                _cartCollection.ReplaceOne(c => c.Id == cart.Id, cart);
            }

            return Ok(cart ?? new CartItem { UserId = userId });
        }
    }
}
