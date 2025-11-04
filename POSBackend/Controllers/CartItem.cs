using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CartItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("userId")]
    public string UserId { get; set; }  // The logged-in user's ID

    [BsonElement("productId")]
    public string ProductId { get; set; }

    [BsonElement("quantity")]
    public int Quantity { get; set; } = 1;
}
