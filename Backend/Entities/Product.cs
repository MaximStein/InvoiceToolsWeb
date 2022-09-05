using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Backend.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id;

        public string Name { get; set; }

        public long GrossPriceMin;

        public long GrossPriceMax;

        public String Title;

        public String ProductNumber;

        public String ImageUrls;


        public String CustomData;
    }
}
