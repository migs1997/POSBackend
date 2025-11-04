using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("userId")]
        public required string UserId { get; set; }

        [BsonElement("productId")]
        public required string ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; } = 1; // default to 1
    }
}
