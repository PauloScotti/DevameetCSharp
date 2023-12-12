namespace DevameetCSharp.Models
{
    public class Meet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
        public ICollection<MeetObjects> MeetObjects { get; set; }
    }
}
