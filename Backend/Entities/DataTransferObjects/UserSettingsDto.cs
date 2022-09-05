namespace Backend.Entities.DataTransferObjects
{
    public class UserSettingsDto
    {
        public string? Email { get; set; }

        public double? DefaultSalesTaxPercent { get; set; }

        public string? InvoiceIssuerAddressLine { get; set; }

        public string? InvoiceBodyText { get; set; }

        public string? InvoiceFooterText { get; set; }

        public long NextInvoiceNumber { get; set; }

    }
}
