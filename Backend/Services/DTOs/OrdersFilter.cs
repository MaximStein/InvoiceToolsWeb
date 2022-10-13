namespace Backend.Services.DTOs
{
    public class OrdersFilter
    {       
    //    public string UserId { get; set; }
        public string[] ShopIds { get; set; }                

        public bool? IsPaid { get; set; }

        public bool? HasInvoice { get; set; }

        public DateTime? MaxDateCreated { get; set; }

        public DateTime? MinDateCreated { get; set; }
    }
}
