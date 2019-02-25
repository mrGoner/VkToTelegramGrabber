using Microsoft.EntityFrameworkCore;

namespace VkGrabber.DataLayer
{
    internal class GrabberDbContext : DbContext
    {
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbGroup> Groups { get; set; }

        public GrabberDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder _optionsBuilder)
        {
            _optionsBuilder.UseSqlite("Data Source=grabber.db");
        }
    }
}