using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Entities
{
    public class BankInfo
    {
        public string? Institute { get; set; }

        public string? Iban { get; set; }
        public string? Bic  { get; set; }

        public string? AccountHolder { get; set; }

    }
}
