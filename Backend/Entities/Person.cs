using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Entities
{
    public class Person
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Id { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Website { get; set; }

        public string? UserAccountName { get; set; }

        public string? Email { get; set; }
    }
}
