namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Shipment
    {
        public long receipt_shipping_id { get; set; }
        public long shipment_notification_timestamp { get; set; }
        public string carrier_name { get; set; }
        public string tracking_code { get; set; }
    }
}
