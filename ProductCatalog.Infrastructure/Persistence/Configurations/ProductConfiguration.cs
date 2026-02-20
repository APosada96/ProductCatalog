using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");

        b.HasKey(x => x.Id);

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        var skuConverter = new ValueConverter<Sku, string>(
            v => v.Value,
            v => new Sku(v));

        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount,
            v => new Money(v));

        b.Property(x => x.Sku)
            .HasConversion(skuConverter)
            .HasMaxLength(32)
            .IsRequired();

        b.HasIndex(x => x.Sku)
            .IsUnique()
            .HasDatabaseName("UX_Products_Sku");

        b.Property(x => x.SalePrice)
            .HasConversion(moneyConverter)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Cost)
            .HasConversion(moneyConverter)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Stock)
            .IsRequired();

        b.Property(x => x.PriceLocked)
            .IsRequired();
    }
}
