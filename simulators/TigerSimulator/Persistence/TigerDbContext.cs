using Microsoft.EntityFrameworkCore;
using TigerSimulator.Domain;

namespace TigerSimulator.Persistence;

public sealed class TigerDbContext(DbContextOptions<TigerDbContext> options) : DbContext(options) 
{
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TigerDbContext).Assembly);
    }
}