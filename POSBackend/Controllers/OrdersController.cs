using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using POSBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orders;

        public OrdersController(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("mobapp");
            _orders = database.GetCollection<Order>("orders");
        }

        // ✅ Place order (POST)
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            try
            {
                if (order == null)
                    return BadRequest(new { success = false, message = "Invalid order data" });

                // Validate delivery fields if DELIVERY
                if (order.DeliveryOption?.ToUpper() == "DELIVERY")
                {
                    var missing = new List<string>();
                    if (string.IsNullOrEmpty(order.Barangay)) missing.Add("Barangay");
                    if (string.IsNullOrEmpty(order.City)) missing.Add("City");
                    if (string.IsNullOrEmpty(order.Province)) missing.Add("Province");
                    if (string.IsNullOrEmpty(order.ContactNumber)) missing.Add("Contact Number");

                    if (missing.Count > 0)
                        return BadRequest(new { success = false, message = $"Missing fields: {string.Join(", ", missing)}" });
                }

                order.CreatedAt = DateTime.UtcNow;
                order.Status = "Pending";

                await _orders.InsertOneAsync(order);

                return Ok(new
                {
                    success = true,
                    message = "Order placed successfully!",
                    orderId = order.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // ✅ Get orders for a specific user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(string userId)
        {
            try
            {
                // Fetch orders for the user
                var orders = await _orders
                    .Find(o => o.UserId == userId)
                    .Project(o => new Order
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        Status = o.Status,
                        CreatedAt = o.CreatedAt,
                        UpdatedAt = o.UpdatedAt,
                        Total = o.Total,
                        PaymentMethod = o.PaymentMethod,
                        DeliveryOption = o.DeliveryOption,
                        
                        ContactNumber = o.ContactNumber,
                        Barangay = o.Barangay,
                        City = o.City,
                        Province = o.Province,
                        
                        // ✅ Ensure Items is never null
                        Items = o.Items ?? new List<OrderItem>()
                    })
                    .ToListAsync();

                // Always return a list (empty if no orders)
                return Ok(orders ?? new List<Order>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }



        // ✅ Cancel order
        [HttpPatch("{userId}/{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(string userId, string orderId)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.Id, orderId) &
                             Builders<Order>.Filter.Eq(o => o.UserId, userId);

                var update = Builders<Order>.Update
                    .Set(o => o.Status, "Cancelled")
                    .Set(o => o.UpdatedAt, DateTime.UtcNow);

                var result = await _orders.UpdateOneAsync(filter, update);

                if (result.ModifiedCount == 0)
                    return NotFound(new { success = false, message = "Order not found or already cancelled" });

                return Ok(new { success = true, message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
