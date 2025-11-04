using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace POSBackend.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // Nullable

        [BsonElement("prod_desc")]
        public string? ProdDesc { get; set; } // Nullable

        [BsonElement("prod_unit_price")]
        public double ProdUnitPrice { get; set; }

        [BsonElement("image_url")]
        public string? ImageUrl { get; set; } // Nullable

        [BsonElement("prod_category")]
        public string? ProdCategory { get; set; } // Nullable
    }
}
