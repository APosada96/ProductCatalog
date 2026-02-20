using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.UnitTests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Constructor_Should_Throw_When_SalePrice_Less_Than_Cost()
    {
        Assert.Throws<InvariantViolationException>(() =>
            new Product(
                Guid.NewGuid(),
                "Test",
                new Sku("ABC-1"),
                new Money(50m),
                new Money(100m),
                10));
    }

    [Fact]
    public void Create_Should_Add_ProductCreated_Event_Once()
    {
        var product = new Product(
            Guid.NewGuid(),
            "Test",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            10);

        // en tu ctor agregas ProductCreated
        Assert.Single(product.Events);
        Assert.IsType<ProductCreated>(product.Events.Single());
    }

    [Fact]
    public void AdjustStock_Should_Throw_When_Resulting_Stock_Negative()
    {
        var product = new Product(
            Guid.NewGuid(),
            "Test",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            10);

        Assert.Throws<InvariantViolationException>(() => product.AdjustStock(-20));
    }

    [Fact]
    public void ApplySafely_Should_Rollback_State_When_Rule_Is_Violated()
    {
        var product = new Product(
            Guid.NewGuid(),
            "Original",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            10);

        // guardamos snapshot "externo" para comparar
        var nameBefore = product.Name;
        var saleBefore = product.SalePrice.Amount;
        var costBefore = product.Cost.Amount;
        var stockBefore = product.Stock;

        // Act: fuerza una violación dentro de ApplySafely (salePrice < cost)
        Assert.Throws<InvariantViolationException>(() =>
            product.UpdatePrice(new Money(40m), new Money(50m)));

        // Assert: rollback (estado igual que antes)
        Assert.Equal(nameBefore, product.Name);
        Assert.Equal(saleBefore, product.SalePrice.Amount);
        Assert.Equal(costBefore, product.Cost.Amount);
        Assert.Equal(stockBefore, product.Stock);
    }

    [Fact]
    public void UpdatePrice_Should_Throw_When_PriceLocked_And_Rollback()
    {
        var product = new Product(
            Guid.NewGuid(),
            "Original",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            10);

        product.LockPrice();

        var saleBefore = product.SalePrice.Amount;
        var costBefore = product.Cost.Amount;

        Assert.Throws<InvariantViolationException>(() =>
            product.UpdatePrice(new Money(120m), new Money(60m)));

        // rollback confirma que no cambió
        Assert.Equal(saleBefore, product.SalePrice.Amount);
        Assert.Equal(costBefore, product.Cost.Amount);
    }

    
}