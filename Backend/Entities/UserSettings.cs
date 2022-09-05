namespace Backend.Entities
{
    public class UserSettings
    {
                
        public double? DefaultSalesTaxPercent { get; set; }

        public string? InvoiceIssuerAddressLine { get; set; }

        public string? InvoiceBodyText { get; set; }

        public string? InvoiceFooterText { get; set; }

        public long NextInvoiceNumber { get; set; }
    }
}
