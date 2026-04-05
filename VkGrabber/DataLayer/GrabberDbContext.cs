using Microsoft.EntityFrameworkCore;
using System.IO;

namespace VkGrabber.DataLayer;

public sealed class GrabberDbContext : DbContext
{
    public DbSet<DbUser> DbUsers { get; set; }
    public DbSet<DbGroup> DbGroups { get; set; }
    private readonly string _pathToDb;

    public GrabberDbContext(string pathToDb)
    {
        _pathToDb = pathToDb;
        Database.EnsureCreated();
        ChangeTracker.AutoDetectChangesEnabled = true;
        ChangeTracker.LazyLoadingEnabled = false;
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(_pathToDb, "grabber.db")}");
    }
}