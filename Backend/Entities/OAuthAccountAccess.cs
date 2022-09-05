using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Backend.Entities
{
    public class OAuthAccountAccess
    {
        //[BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string? Id { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }  

        public DateTime RefreshTokenExpiration { get; set; }    

        public string? TokenSecret { get; set; }

        public bool IsTokenInvalid { get; set; } = false;

        public bool IsActive { get; set; } = true;



        [BsonRepresentation(BsonType.String)]
        public Enums.ShopApiType ShopApiType { get; set; }
    }
}
