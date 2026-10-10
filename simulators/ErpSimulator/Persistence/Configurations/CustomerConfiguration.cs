using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ErpSimulator.Domain;

namespace ErpSimulator.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Address).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Email).HasMaxLength(100);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();
    }
}