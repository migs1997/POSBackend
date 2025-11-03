using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace POSBackend.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
        public string Note { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Blk { get; set; } = string.Empty;
        public string Lot { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
        public string DeliveryOption { get; set; } = "DELIVERY";
        public string PaymentMethod { get; set; } = "CASH";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class OrderItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
