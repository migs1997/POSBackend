using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _products;

        public ProductController()
        {
            var client = new MongoClient("mongodb+srv://MobApp:admin123@cluster0.k28aler.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            var database = client.GetDatabase("mobapp");
            _products = database.GetCollection<BsonDocument>("products");
        }

        // ✅ Return all products with prod_qty
        [HttpGet("all")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _products.Find(new BsonDocument()).ToListAsync();
                var baseUrl = "https://posbackend-1-o9uk.onrender.com"; // 👈 use your live domain

                var result = products.ConvertAll(product =>
                {
                    var id = product.Contains("_id") && product["_id"] != null
                        ? product["_id"].ToString()!
                        : Guid.NewGuid().ToString();

                    var prodDesc = product.Contains("prod_desc") && product["prod_desc"] != null
                        ? product["prod_desc"].AsString
                        : "Unnamed Product";

                    var productDesc = product.Contains("product_desc") && product["product_desc"] != null
                        ? product["product_desc"].AsString
                        : "";

                    var prodUnitPrice = product.Contains("prod_unit_price") && product["prod_unit_price"] != null
                        ? product["prod_unit_price"].ToString()
                        : "0";

                    var prodCategory = product.Contains("prod_category") && product["prod_category"] != null
                        ? product["prod_category"].AsString
                        : "Uncategorized";

                    var prodDescExtra = product.Contains("prod_desc_extra") && product["prod_desc_extra"] != null
                        ? product["prod_desc_extra"].AsString
                        : "";

                    // ✅ prod_qty field
                    var prodQty = product.Contains("prod_qty") && product["prod_qty"] != null
                        ? product["prod_qty"].AsInt32
                        : 0;

                    string imageUrl = $"{baseUrl}/api/Product/image/{id}";

                    return new
                    {
                        _id = id,
                        prod_desc = prodDesc,
                        product_desc = productDesc,
                        prod_unit_price = prodUnitPrice,
                        prod_category = prodCategory,
                        prod_desc_extra = prodDescExtra,
                        prod_qty = prodQty, // ✅ included prod_qty
                        image_url = imageUrl
                    };
                });

                return Ok(new { success = true, products = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error fetching products: {ex.Message}" });
            }
        }

        // ✅ Fetch single product image
        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetProductImage(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out ObjectId objId))
                    return BadRequest("Invalid product ID.");

                var filter = Builders<BsonDocument>.Filter.Eq("_id", objId);
                var product = await _products.Find(filter).FirstOrDefaultAsync();

                if (product == null)
                    return NotFound("Product not found.");

                // Try possible image fields
                string[] possibleFields = { "ImageData", "product_image", "image", "photo" };
                BsonValue? imageData = null;

                foreach (var field in possibleFields)
                {
                    if (product.Contains(field) && product[field] != null)
                    {
                        imageData = product[field];
                        break;
                    }
                }

                if (imageData == null)
                    return NotFound("No image found for this product.");

                byte[] imageBytes;

                if (imageData.IsString)
                    imageBytes = Convert.FromBase64String(imageData.AsString ?? "");
                else if (imageData.IsBsonBinaryData)
                    imageBytes = imageData.AsBsonBinaryData?.Bytes ?? Array.Empty<byte>();
                else
                    return BadRequest("Unsupported image format.");

                return File(imageBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching image: {ex.Message}");
            }
        }

        // ✅ Place an order and reduce stock
        [HttpPost("order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequest order)
        {
            try
            {
                if (!ObjectId.TryParse(order.ProductId, out ObjectId objId))
                    return BadRequest(new { success = false, message = "Invalid product ID." });

                var filter = Builders<BsonDocument>.Filter.Eq("_id", objId);
                var product = await _products.Find(filter).FirstOrDefaultAsync();

                if (product == null)
                    return NotFound(new { success = false, message = "Product not found." });

                int currentQty = product.Contains("prod_qty") ? product["prod_qty"].ToInt32() : 0;

                if (currentQty < order.Quantity)
                    return BadRequest(new { success = false, message = "Not enough stock available." });

                int newQty = currentQty - order.Quantity;

                var update = Builders<BsonDocument>.Update.Set("prod_qty", newQty);
                await _products.UpdateOneAsync(filter, update);

                return Ok(new { success = true, message = "Order placed successfully.", remaining_stock = newQty });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error placing order: {ex.Message}" });
            }
        }
    }

    // DTO for order
    public class OrderRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
