namespace Backend.Services.ShopApis.Etsy.Types
{
    public class ListResult<T>
    {
        public long count { get; set; }
        public List<T> results { get; set; }
    }
}
