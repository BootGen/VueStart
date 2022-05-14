using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VueStart;

public class ApplicationDbContext : DbContext
{
    public DbSet<InputData> InputData { get; set; }
    public DbSet<StatisticRecord> StatisticRecords { get; set; }
    public DbSet<ProfilerRecord> ProfilerRecords { get; set; }
    public DbSet<ServerError> ServerErrors { get; set; }
    public DbSet<ClientError> ClientErrors { get; set; }
    public DbSet<Visitor> Visitors { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ShareableLink> ShareableLinks { get; set; }
    public IConfiguration Configuration { get; }

    
    public ApplicationDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.Entity<InputData>()
            .Property(b => b.Hash)
            .IsRequired();
        modelBuilder.Entity<InputData>()
            .Property(b => b.Data)
            .IsRequired();
        modelBuilder.Entity<Visitor>()
            .Property(b => b.Token)
            .IsRequired();
        modelBuilder.Entity<Visitor>()
            .HasMany<Visit>(v => v.Visits);
        modelBuilder.Entity<InputData>()
            .HasMany<StatisticRecord>(v => v.StatisticRecords);
        modelBuilder.Entity<StatisticRecord>()
            .HasOne<InputData>(v => v.InputData);
        
        var passwordHasher = new PasswordHasher<User>();
        var admin = new User {
            Id = 1,
            Username = "admin"
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "password");
        modelBuilder.Entity<User>().HasData(admin);
    }
}