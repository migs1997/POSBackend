using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CartItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }  // optional, MongoDB will generate

    [BsonElement("userId")]
    public string? UserId { get; set; }  // optional in POST, set in controller

    [BsonElement("productId")]
    public string ProductId { get; set; } = null!;

    [BsonElement("quantity")]
    public int Quantity { get; set; } = 1;
}
