using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TigerSimulator.Domain;

namespace TigerSimulator.Persistence.Configurations;

public sealed class SalesOrderLineConfiguration : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLine", table =>
        {
            table.HasCheckConstraint("CK_SalesOrderLine_Quantity_Positive", "[Quantity] > 0");
            table.HasCheckConstraint("CK_SalesOrderLine_UnitPrice_NonNegative", "[UnitPrice] >= 0");
            table.HasCheckConstraint("CK_SalesOrderLine_LineNumber_Positive", "[LineNumber] > 0");
        });

        builder.HasKey(sol => sol.Id);

        builder.Property(sol => sol.Quantity).HasColumnType("decimal(18,3)").IsRequired();
        builder.Property(sol => sol.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

        builder.Ignore(sol => sol.LineTotal);

        builder.HasIndex(sol => new { sol.SalesOrderId, sol.LineNumber }).IsUnique();

        builder.HasOne<Item>().WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Restrict);
    }
}