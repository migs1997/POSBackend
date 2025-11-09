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
        public required string ProdDesc { get; set; }

        [BsonElement("prod_unit_price")]
        public decimal ProdUnitPrice { get; set; }

        [BsonElement("image_url")]
        public required string ImageUrl { get; set; }

        [BsonElement("prod_category")]
        public required string ProdCategory { get; set; }

        [BsonElement("prod_desc_extra")]
        public string? ProdDescExtra { get; set; }

        // ✅ Add prod_qty for stock
        [BsonElement("prod_qty")]
        public int ProdQty { get; set; } = 0; // default 0

        // ✅ Add bestseller field
        [BsonElement("is_bestseller")]
        public bool IsBestseller { get; set; } = false; // default false
    }
}
