using ProductCatalog.Domain.ValueObjects;
using Xunit;

namespace ProductCatalog.UnitTests.Domain;


public class MoneyTests
{
    [Fact]
    public void Should_Not_Allow_Negative_Value()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-10m));
    }

    [Fact]
    public void Equality_Should_Be_Structural()
    {
        var m1 = new Money(100);
        var m2 = new Money(100);

        Assert.Equal(m1, m2);
    }
}

