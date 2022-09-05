namespace Backend.Entities.DataTransferObjects
{
    public class OrderDto
    {

        public string? Id { get; set; }

        public string? ShopId { get; set; }

        public Person? Buyer { get; set; }

        public DateTime TimeOrdered { get; set; }

        public DateTime? TimeUpdated { get; set; }

        public DateTime? TimeShipped { get; set; }

       //public DateTime? TimePaid { get; set; }
        public bool? IsPaid { get; set; }
        public long? InvoiceNumber { get; set; }

        public DateTime? InvoiceDate { get; set; }
        public string? ShippingAddress { get; set; }

        public string? BillingsAddress { get; set; }

        public string? OrderNumber { get; set; }


        //public long TotalGrossAmount { get; set; }

        public long? TotalFees { get; set; }

        public string? BuyerMessage { get; set; }


        public double RefundAmount { get; set; }

        //public double VatPercent { get; set; } = 19;

        public bool GiftOptionsIsGift = false;

        public string? GiftOptionsGiftMessage = null;
        
        public string CurrencyCode { get; set; }

        public List<LineItemDto> Items { get; set; } = new();

        public bool? IsCancelled { get; set; }

        public string? ImportedFrom { get; set; }

        public double GrandTotal { get => Items.Sum(i => i.Quantity * i.Price) - RefundAmount; }
    
    }
}
