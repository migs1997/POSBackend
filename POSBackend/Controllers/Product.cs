using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("prod_desc")]
        public string? ProdDesc { get; set; }

        [BsonElement("prod_unit_price")]
        public decimal ProdUnitPrice { get; set; } = 0;

        [BsonElement("image_url")]
        public string? ImageUrl { get; set; }

        [BsonElement("prod_category")]
        public string? ProdCategory { get; set; }
    }
}
