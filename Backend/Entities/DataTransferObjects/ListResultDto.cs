namespace Backend.Entities.DataTransferObjects
{
    public class ListResultDto<T>
    {
        public List<T> Items{ get; set; }
        public long TotalItemsCount { get; set; }
    }
}
