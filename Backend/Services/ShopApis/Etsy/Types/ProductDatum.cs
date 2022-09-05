namespace Backend.Services.ShopApis.Etsy.Types
{
    public class ProductDatum
    {
        public long property_id { get; set; }
        public string property_name { get; set; }
        public long? scale_id { get; set; }
        public string scale_name { get; set; }
        public List<long> value_ids { get; set; }
        public List<string> values { get; set; }
    }
}
