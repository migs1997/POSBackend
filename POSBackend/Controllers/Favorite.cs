using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace POSBackend.Models
{
    public class Favorite
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; } = string.Empty;  // Initialized to avoid CS8618

        [BsonElement("products")]
        public List<string> Products { get; set; } = new List<string>();
    }
}
