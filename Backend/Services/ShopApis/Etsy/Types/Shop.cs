namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Shop
    {
        public long shop_id { get; set; }
        public string shop_name { get; set; }
        public long user_id { get; set; }
        public long create_date { get; set; }
        public long created_timestamp { get; set; }
        public object title { get; set; }
        public object announcement { get; set; }
        public string currency_code { get; set; }
        public bool is_vacation { get; set; }
        public object vacation_message { get; set; }
        public object sale_message { get; set; }
        public object digital_sale_message { get; set; }
        public long update_date { get; set; }
        public long updated_timestamp { get; set; }
        public long listing_active_count { get; set; }
        public long digital_listing_count { get; set; }
        public string login_name { get; set; }
        public bool accepts_custom_requests { get; set; }
        public object vacation_autoreply { get; set; }
        public string url { get; set; }
        public object image_url_760x100 { get; set; }
        public long num_favorers { get; set; }
        public List<string> languages { get; set; }
        public object icon_url_fullxfull { get; set; }
        public bool is_using_structured_policies { get; set; }
        public bool has_onboarded_structured_policies { get; set; }
        public bool include_dispute_form_link { get; set; }
        public bool is_direct_checkout_onboarded { get; set; }
        public bool is_etsy_payments_onboarded { get; set; }
        public bool is_opted_in_to_buyer_promise { get; set; }
        public bool is_calculated_eligible { get; set; }
        public bool is_shop_us_based { get; set; }
        public long transaction_sold_count { get; set; }
        public string shipping_from_country_iso { get; set; }
        public object shop_location_country_iso { get; set; }
        public object policy_welcome { get; set; }
        public string policy_payment { get; set; }
        public string policy_shipping { get; set; }
        public string policy_refunds { get; set; }
        public string policy_additional { get; set; }
        public string policy_seller_info { get; set; }
        public long policy_update_date { get; set; }
        public bool policy_has_private_receipt_info { get; set; }
        public bool has_unstructured_policies { get; set; }
        public object policy_privacy { get; set; }
        public double review_average { get; set; }
        public long review_count { get; set; }
    }
}
