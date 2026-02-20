using AutoMapper;
using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetProducts;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;
using CachingDataSource = ProductCatalog.Application.Abstractions.Caching.DataSource;

namespace ProductCatalog.UnitTests.Application;
public sealed class GetProductsHandlerTests
{
   
    [Fact]
    public async Task When_Repo_Returns_Empty_Should_Return_Empty_Page()
    {
        // Arrange
        var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
        var mapper = new Mock<IMapper>(MockBehavior.Strict);

        var q = new GetProductsQuery(
            PageNumber: 1,
            PageSize: 10,
            SortField: null,
            SortDirection: null);

        readRepo
            .Setup(x => x.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        var handler = new GetProductsHandler(readRepo.Object, mapper.Object);

        // Act
        var result = await handler.Handle(q, default);

        // Assert
        Assert.Equal(CachingDataSource.Db, result.Source);
        Assert.Equal(0, result.ElapsedMs);
        Assert.NotNull(result.Data.Items);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.Total);

        // mapper nunca se llama si no hay items
        mapper.Verify(m => m.Map<ProductDto>(It.IsAny<Product>()), Times.Never);

        readRepo.VerifyAll();
        mapper.VerifyAll();
    }

    private static decimal Margin(Product p)
    {
        if (p.SalePrice.Amount <= 0) return 0m;
        return decimal.Round(((p.SalePrice.Amount - p.Cost.Amount) / p.SalePrice.Amount) * 100m, 2);
    }
}

