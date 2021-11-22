using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VueStart
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<StatisticRecord> StatisticRecords { get; set; }
        public DbSet<ProfilerRecord> ProfilerRecords { get; set; }
        public DbSet<ServerError> ServerErrors { get; set; }
        public DbSet<ClientError> ClientErrors { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Visit> Visits { get; set; }
        public IConfiguration Configuration { get; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Configuration.GetConnectionString("MySQL");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.23"));
            } else {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                optionsBuilder.UseSqlite(connection);
            }
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
            modelBuilder.Entity<Visit>()
                .Property(b => b.Id)
                .IsRequired();
            modelBuilder.Entity<ProfilerRecord>()
                .Property(b => b.Id)
                .IsRequired();
            modelBuilder.Entity<ServerError>()
                .Property(b => b.Id)
                .IsRequired();
        }
    }
}