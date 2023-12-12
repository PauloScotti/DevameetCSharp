namespace DevameetCSharp.Dtos
{
    public class MeetUpdateRequestDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public List<MeetObjectsDto> Objects { get; set; }
    }
}
