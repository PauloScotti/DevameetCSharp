using System.ComponentModel.DataAnnotations.Schema;

namespace DevameetCSharp.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string ClientId { get; set;}
        public int X {  get; set; }
        public int Y { get; set; }
        public string Orientation { get; set; }
        public bool Muted { get; set; }

        [ForeignKey("Meet")]
        public int MeetId { get; set; }
        public Meet meet { get; set; }
    }
}
