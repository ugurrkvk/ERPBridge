using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TigerSimulator.Domain;

namespace TigerSimulator.Persistence.Configurations;

public sealed class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Item", table =>
        {
            table.HasCheckConstraint("CK_Item_UnitPrice_NonNegative", "[UnitPrice] >= 0");
            table.HasCheckConstraint("CK_Item_OnHandQuantity_NonNegative", "[OnHandQuantity] >= 0");
            table.HasCheckConstraint("CK_Item_ReservedQuantity_NonNegative", "[ReservedQuantity] >= 0");
            table.HasCheckConstraint("CK_Item_ReservedQuantity_NotAboveStock", "[ReservedQuantity] <= [OnHandQuantity]");
        });

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Code).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(200);
        builder.Property(i => i.Barcode).IsRequired().HasMaxLength(50);
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(i => i.OnHandQuantity).HasColumnType("decimal(18,3)");
        builder.Property(i => i.ReservedQuantity).HasColumnType("decimal(18,3)");
        builder.Property(i=>i.IsActive).IsRequired();
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.Ignore(i => i.AvailableQuantity);

        builder.HasIndex(i => i.Code).IsUnique();
        builder.HasIndex(i => i.Barcode).IsUnique();
    }
}