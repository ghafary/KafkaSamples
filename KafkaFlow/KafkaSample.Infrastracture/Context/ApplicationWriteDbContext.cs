using KafkaSample.Domain.AggregatesModel.CustomerAggregate;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace KafkaSample.Framework.Infrastructure;

public class ApplicationWriteDbContext : DbContext
{
    public DbSet<Transaction> Transactions { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=KafkaSample.db");
}