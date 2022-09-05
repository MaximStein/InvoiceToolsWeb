namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Refund
    {
        public Amount amount { get; set; }
        public long created_timestamp { get; set; }
        public string reason { get; set; }
        public string note_from_issuer { get; set; }
        public string status { get; set; }
    }
}
