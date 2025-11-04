using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using POSBackend.Models;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IMongoCollection<CartItem> _cartCollection;

    public CartController(IMongoClient client)
    {
        var database = client.GetDatabase("POS");
        _cartCollection = database.GetCollection<CartItem>("CartItems");
    }

    // Add item to cart
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromQuery] string userId, [FromBody] CartItem item)
    {
        if (string.IsNullOrEmpty(userId) || item == null || string.IsNullOrEmpty(item.ProductId))
            return BadRequest(new { success = false, message = "Invalid request" });

        // Assign userId from query
        item.UserId = userId;

        // Check if item already exists for this user
        var filter = Builders<CartItem>.Filter.Eq("UserId", userId) &
                     Builders<CartItem>.Filter.Eq("ProductId", item.ProductId);

        var existing = await _cartCollection.Find(filter).FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.Quantity += item.Quantity;
            await _cartCollection.ReplaceOneAsync(filter, existing);
        }
        else
        {
            await _cartCollection.InsertOneAsync(item);
        }

        return Ok(new { success = true, message = "Item added to cart" });
    }

    // Get user's cart
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var cart = await _cartCollection.Find(c => c.UserId == userId).ToListAsync();
        return Ok(new { success = true, cart });
    }

    // Optional: remove item from cart
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFromCart([FromQuery] string userId, [FromQuery] string productId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
            return BadRequest(new { success = false, message = "Invalid request" });

        var filter = Builders<CartItem>.Filter.Eq("UserId", userId) &
                     Builders<CartItem>.Filter.Eq("ProductId", productId);

        await _cartCollection.DeleteOneAsync(filter);
        return Ok(new { success = true, message = "Item removed from cart" });
    }
}
