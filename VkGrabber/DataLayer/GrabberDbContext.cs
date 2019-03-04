using Microsoft.EntityFrameworkCore;

namespace VkGrabber.DataLayer
{
    public class GrabberDbContext : DbContext
    {
        public DbSet<DbUser> DbUsers { get; set; }
        public DbSet<DbGroup> DbGroups { get; set; }

        public GrabberDbContext()
        {
            Database.EnsureCreated();
            ChangeTracker.AutoDetectChangesEnabled = true;
            ChangeTracker.LazyLoadingEnabled = false;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder _optionsBuilder)
        {
            _optionsBuilder.UseSqlite("Data Source=grabber.db");
        }
    }
}