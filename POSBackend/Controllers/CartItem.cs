using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace POSBackend.Models
{
    public class CartItem
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;  // Initialized to avoid CS8618

        [BsonElement("items")]
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class Item
    {
        [BsonElement("product_id")]
        public string ProductId { get; set; } = string.Empty;  // Initialized to avoid CS8618

        [BsonElement("quantity")]
        public int Quantity { get; set; }

        [BsonElement("added_at")]
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
