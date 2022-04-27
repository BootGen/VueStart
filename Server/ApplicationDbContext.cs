using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VueStart;

public class ApplicationDbContext : DbContext
{
    public DbSet<StatisticRecord> StatisticRecords { get; set; }
    public DbSet<ProfilerRecord> ProfilerRecords { get; set; }
    public DbSet<ServerError> ServerErrors { get; set; }
    public DbSet<ClientError> ClientErrors { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<ShareableLink> ShareableLinks { get; set; }
    public IConfiguration Configuration { get; }

    
    public ApplicationDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            Console.WriteLine("Using PostgreSQL");
            optionsBuilder.UseNpgsql(connectionString);
        } else {
            Console.WriteLine("Using Sqlite");
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
        modelBuilder.Entity<ShareableLink>()
            .Property(b => b.Hash)
            .IsRequired();
        modelBuilder.Entity<ShareableLink>()
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