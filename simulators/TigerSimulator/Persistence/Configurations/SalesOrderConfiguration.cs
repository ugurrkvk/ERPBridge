using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TigerSimulator.Domain;

namespace TigerSimulator.Persistence.Configurations;

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrder");

        builder.HasKey(so => so.Id);

        builder.Property(so => so.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(so => so.SourceSystem).IsRequired().HasMaxLength(30);
        builder.Property(so => so.SourceSellerId).IsRequired().HasMaxLength(50);
        builder.Property(so => so.ExternalOrderId).IsRequired().HasMaxLength(100);
        builder.Property(so => so.RequestHash).IsRequired().HasMaxLength(64).IsFixedLength();
        builder.Property(so => so.Currency).IsRequired().HasMaxLength(3);
        builder.Property(so => so.OrderDateUtc).HasColumnType("datetime2");
        builder.Property(so => so.CreatedAtUtc).HasColumnType("datetime2");

        builder.Ignore(so => so.TotalAmount);

        builder.HasIndex(so => so.OrderNumber).IsUnique();
        builder.HasIndex(so => new { so.SourceSystem, so.SourceSellerId, so.ExternalOrderId }).IsUnique();

        builder.HasOne<Customer>().WithMany().HasForeignKey(so => so.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(ol => ol.SalesOrderId).OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Lines).HasField("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}