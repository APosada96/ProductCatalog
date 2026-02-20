using ProductCatalog.Domain.ValueObjects;


namespace ProductCatalog.UnitTests.Domain;
public class SkuTests
{
    [Fact]
    public void Should_Normalize_Sku()
    {
        var sku = new Sku(" ab-0o1 ");

        Assert.Equal("AB-001", sku.Value);
    }

    [Fact]
    public void Equality_Should_Work()
    {
        var s1 = new Sku("abc-123");
        var s2 = new Sku(" ABC-123 ");

        Assert.Equal(s1, s2);
    }
}
