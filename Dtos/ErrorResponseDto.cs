namespace DevameetCSharp.Dtos
{
    public class ErrorResponseDto
    {
        public int Status { get; set; }
        public string Description { get; set; }
        public List<string> Errors { get; set; }
    }
}
