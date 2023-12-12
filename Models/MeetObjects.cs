namespace DevameetCSharp.Models
{
    public class MeetObjects
    {
        public int Id { get; set; }
        public int MeetId { get; set; }
        public string Name { get; set;}
        public int X { get; set;}
        public int Y { get; set;}
        public int ZIndex { get; set;}
        public bool Walkable { get; set; }
        public string Orientation { get; set;}
    }
}
