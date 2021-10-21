using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace VueStart
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<StatisticRecord> StatisticRecords { get; set; }
        public DbSet<Visitor> Visitors { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatisticRecord>()
                .Property(b => b.Hash)
                .IsRequired();
            modelBuilder.Entity<StatisticRecord>()
                .Property(b => b.Data)
                .IsRequired();
            modelBuilder.Entity<Visitor>()
                .Property(b => b.Token)
                .IsRequired();
            modelBuilder.Entity<Visitor>()
                .HasMany<Visit>(v => v.Visits);
        }
    }
}