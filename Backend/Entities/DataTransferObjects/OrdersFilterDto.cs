namespace Backend.Entities.DataTransferObjects
{
    public class OrdersFilterDto
    {
        //public string? UserId { get; set; }

        //    public DateTime StartTime { get; set; }

        //    public DateTime EndTime { get; set; }

        public string[] ShopIds { get; set; } = { };

        public bool OnlyPaid { get; set; }

        public bool OnlyWithoutInvoice { get; set; }

    }
}
