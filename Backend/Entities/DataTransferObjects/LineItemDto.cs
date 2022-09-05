namespace Backend.Entities.DataTransferObjects
{
    public class LineItemDto
    {
        public string Title { get; set; }

        //public string? Description { get; set; }

        //public string? BuyerMessage { get; set; }

        public double Price { get; set; }

        public long Quantity { get; set; }

        public string? Variation { get; set; }

        public string? ImageFile { get; set; }
    }
}
