using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Backend.Entities
{
    [BsonIgnoreExtraElements]
    [CollectionName("Orders")]
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string OrderNumber { get; set; }

        //public ObjectId UserId {get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string? ShopId { get; set; }

        public Person? Buyer { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime TimeOrdered { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? TimeShipped { get; set; }

        public string? ShippingAddress { get; set; }

        public string? BillingsAddress { get; set; }

        public long? InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }

        //public long TotalGrossAmount { get; set; }

        public double? TotalFees { get; set; }

        public string? BuyerMessage { get; set; }

        public bool? IsPaid { get; set; }

        public string? PaymentMethod { get; set; } 

        public double? RefundAmount { get; set; }

       // public double VatPercent { get; set; } = 19;

        public OrderGiftOptions? GiftOptions { get; set; } = null;

        [BsonRepresentation(BsonType.String)]
        public Enums.CurrencyCode CurrencyCode { get; set; } = Enums.CurrencyCode.EUR;

        public List<LineItem> Items { get; set; } = new ();

        public bool? IsCancelled { get; set; }


        public Enums.ShopApiType? ImportedFrom { get; set; }
    }
}
