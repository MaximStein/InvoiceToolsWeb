namespace Backend.Entities
{
    public class ImportResult
    {
        public int OrdersCreated { get; set; }

        public int OrdersUpdated { get; set; }

        public DateTime Time { get; set; }

        public bool Successful { get; set; }
    }
}
