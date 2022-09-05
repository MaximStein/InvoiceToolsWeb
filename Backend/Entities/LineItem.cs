using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



namespace Backend.Entities
{
    [BsonIgnoreExtraElements]
    public class LineItem
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Id;

        //public Product Product;

        public string Title { get; set; }

        public string? Description { get; set; } 

        //public string? BuyerMessage { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

        public string? Variation { get; set; }

        public string? ImageFile { get; set; }
    }
}
