using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using Microsoft.EntityFrameworkCore;

namespace KafkaSample.Framework.Infrastructure;

public class ApplicationReadDbContext : DbContext
{
    public IQueryable<Transaction> Transactions => Set<Transaction>();
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=KafkaSample.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>().ToTable("Transactions");
        base.OnModelCreating(modelBuilder);
    }
}