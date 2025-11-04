using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

        [BsonElement("prod_desc")]
        public required string ProdDesc { get; set; }

        [BsonElement("prod_unit_price")]
        public decimal ProdUnitPrice { get; set; }

        [BsonElement("image_url")]
        public required string ImageUrl { get; set; }

        [BsonElement("prod_category")]
        public required string ProdCategory { get; set; }
    }
}
