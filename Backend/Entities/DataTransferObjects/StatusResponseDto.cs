namespace Backend.DTOs
{
    public class StatusResponseDto
    {
        public bool IsSuccessful { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
