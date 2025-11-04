using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // MAKE NULLABLE so client doesn't need to send it

        [BsonElement("userId")]
        public required string UserId { get; set; }

        [BsonElement("productId")]
        public required string ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; } = 1;
    }
}
