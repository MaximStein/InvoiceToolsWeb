using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using System;
namespace Backend.Entities
{
    [BsonIgnoreExtraElements]
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<ObjectId>
    {


        public List<Shop> Shops { get; set; } = new List<Shop>();

        public UserSettings UserSettings { get; set; } = new UserSettings();

        public bool HasShop(string id) => Shops.Any(s => s.Id == id);

        public bool HasShops(string[] shopIds) => shopIds.All(shopId => Shops.Any(usershop => usershop.Id == shopId));
    }
}
