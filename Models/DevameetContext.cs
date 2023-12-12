using Microsoft.EntityFrameworkCore;

namespace DevameetCSharp.Models
{
    public class DevameetContext : DbContext
    {
        public DevameetContext(DbContextOptions<DevameetContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Meet> Meets { get; set; }
        public DbSet<MeetObjects> MeetObjects { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
    