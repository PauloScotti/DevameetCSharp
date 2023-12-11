using Microsoft.EntityFrameworkCore;

namespace DevameetCSharp.Models
{
    public class DevameetContext : DbContext
    {
        public DevameetContext(DbContextOptions<DevameetContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
