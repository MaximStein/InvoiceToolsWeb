namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Receipt
    {
        public long receipt_id { get; set; }
        public long receipt_type { get; set; }
        public long seller_user_id { get; set; }
        public string seller_email { get; set; }
        public long buyer_user_id { get; set; }
        public string buyer_email { get; set; }
        public string name { get; set; }
        public string first_line { get; set; }
        public string second_line { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string status { get; set; }
        public string formatted_address { get; set; }
        public string country_iso { get; set; }
        public string payment_method { get; set; }
        public string payment_email { get; set; }
        public string message_from_seller { get; set; }
        public string message_from_buyer { get; set; }
        public string message_from_payment { get; set; }
        public bool is_paid { get; set; }
        public bool is_shipped { get; set; }
        public long? create_timestamp { get; set; }
        public long created_timestamp { get; set; }
        public long? update_timestamp { get; set; }
        public long? updated_timestamp { get; set; }
        public bool is_gift { get; set; }
        public string gift_message { get; set; }
        public Amount grandtotal { get; set; }
        public Amount subtotal { get; set; }
        public Amount total_price { get; set; }
        public Amount total_shipping_cost { get; set; }
        public Amount total_tax_cost { get; set; }
        public Amount total_vat_cost { get; set; }
        public Amount discount_amt { get; set; }
        public Amount gift_wrap_price { get; set; }
        public List<Shipment> shipments { get; set; }
        public List<Transaction> transactions { get; set; }
        public List<Refund> refunds { get; set; }
    }
}
