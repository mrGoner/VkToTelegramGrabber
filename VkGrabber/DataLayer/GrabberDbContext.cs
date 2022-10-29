﻿using Microsoft.EntityFrameworkCore;
using System.IO;

namespace VkGrabber.DataLayer
{
    public class GrabberDbContext : DbContext
    {
        public DbSet<DbUser> DbUsers { get; set; }
        public DbSet<DbGroup> DbGroups { get; set; }
        private readonly string m_pathToDb;

        public GrabberDbContext(string _pathToDb)
        {
            m_pathToDb = _pathToDb;
            Database.EnsureCreated();
            ChangeTracker.AutoDetectChangesEnabled = true;
            ChangeTracker.LazyLoadingEnabled = false;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder _optionsBuilder)
        {
            _optionsBuilder.UseSqlite($"Data Source={Path.Combine(m_pathToDb, "grabber.db")}");
        }

        protected override void OnModelCreating(ModelBuilder _modelBuilder)
        {
            _modelBuilder.Entity<DbGroup>().HasOne(e => e.DbUser);

            _modelBuilder
                .Entity<DbUser>()
                .HasMany(e => e.DbGroups)
                .WithOne(e => e.DbUser)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class DbContextFactory
    {
        private readonly string m_pathToDb;
        public DbContextFactory(string _pathToDb)
        {
            m_pathToDb = _pathToDb;
        }

        public GrabberDbContext CreateContext()
        {
            return new GrabberDbContext(m_pathToDb);
        }
    }
}