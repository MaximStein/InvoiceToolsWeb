namespace Backend.Services.ShopApis.Etsy.Types
{
    public class Variation
    {
        public long? property_id { get; set; }
        public long? value_id { get; set; }
        public string formatted_name { get; set; }
        public string formatted_value { get; set; }
    }
}
