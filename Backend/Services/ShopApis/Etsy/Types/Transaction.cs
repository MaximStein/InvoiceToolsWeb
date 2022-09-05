namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Transaction
    {
        public long transaction_id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public long seller_user_id { get; set; }
        public long buyer_user_id { get; set; }
        public long create_timestamp { get; set; }
        public long created_timestamp { get; set; }
        public long? paid_timestamp { get; set; }
        public long? shipped_timestamp { get; set; }
        public long quantity { get; set; }
        public long listing_image_id { get; set; }
        public long receipt_id { get; set; }
        public bool is_digital { get; set; }
        public string file_data { get; set; }
        public long listing_id { get; set; }
        public string transaction_type { get; set; }
        public long product_id { get; set; }
        public string sku { get; set; }
        public Amount price { get; set; }
        public Amount shipping_cost { get; set; }
        public List<Variation> variations { get; set; }
        public List<ProductDatum> product_data { get; set; }
        public long? shipping_profile_id { get; set; }
        public long? min_processing_days { get; set; }
        public long? max_processing_days { get; set; }
        public string shipping_method { get; set; }
        public string shipping_upgrade { get; set; }
        public long? expected_ship_date { get; set; }
        public long? buyer_coupon { get; set; }
        public long? shop_coupon { get; set; }
    }
}
