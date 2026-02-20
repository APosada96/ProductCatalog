using ProductCatalog.Domain.ValueObjects;
using Xunit;

namespace ProductCatalog.UnitTests.Domain;


public class MoneyTests
{

    [Fact]
    public void Equality_Should_Be_Structural()
    {
        var m1 = new Money(100);
        var m2 = new Money(100);

        Assert.Equal(m1, m2);
    }
}

