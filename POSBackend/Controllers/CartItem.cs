using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("productId")]
        public string? ProductId { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; } = 1;
    }
}
