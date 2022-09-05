using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Entities
{
    public class MoneyAmount
    {
        public long AmountCents;

        [BsonRepresentation(BsonType.String)]
        public Enums.CurrencyCode CurrencyCode;
    }
}
