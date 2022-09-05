using Backend.Services.DTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Backend.Entities
{

    [BsonIgnoreExtraElements]
    public class Shop
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]     
        public string? Id { get; set; }

        public string? Label { get; set; }

        public ImportResult? LastImport { get; set; }

        public OAuthAccountAccess? OAuthAccess { get; set; } = null;

        public Shop()
        {
            Id =  ObjectId.GenerateNewId().ToString();
        }
    }
}
