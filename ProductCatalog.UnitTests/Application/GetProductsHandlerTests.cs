using AutoMapper;
using Moq;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetProducts;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.UnitTests.Application;

using CachingDataSource = ProductCatalog.Application.Abstractions.Caching.DataSource;
using CachingQueryResult =
    ProductCatalog.Application.Abstractions.Caching.QueryResult<
        ProductCatalog.Application.DTOs.PagedResult<ProductCatalog.Application.DTOs.ProductDto>>;

public sealed class GetProductsHandlerTests
{
    [Fact]
    public async Task Should_Call_Repo_With_Paging_And_Sorting_And_Return_PagedResult()
    {
        // Arrange
        var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
        var mapper = new Mock<IMapper>(MockBehavior.Strict);

        var p1 = new Product(
            Guid.NewGuid(),
            "P1",
            new Sku("SKU-1"),
            new Money(120m),
            new Money(60m),
            stock: 10);

        var p2 = new Product(
            Guid.NewGuid(),
            "P2",
            new Sku("SKU-2"),
            new Money(200m),
            new Money(100m),
            stock: 5);

        var items = new List<Product> { p1, p2 };
        const int total = 25;

        var q = new GetProductsQuery(
            PageNumber: 2,
            PageSize: 10,
            SortField: "Name",
            SortDirection: "asc");

        readRepo
            .Setup(x => x.GetPagedAsync(
                q.PageNumber,
                q.PageSize,
                q.SortField,
                q.SortDirection,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((items, total));

        mapper
            .Setup(m => m.Map<ProductDto>(p1))
            .Returns(new ProductDto(p1.Id, p1.Name, p1.Sku.Value, p1.SalePrice.Amount, p1.Cost.Amount, p1.Stock, Margin(p1)));

        mapper
            .Setup(m => m.Map<ProductDto>(p2))
            .Returns(new ProductDto(p2.Id, p2.Name, p2.Sku.Value, p2.SalePrice.Amount, p2.Cost.Amount, p2.Stock, Margin(p2)));

        var handler = new GetProductsHandler(readRepo.Object, mapper.Object);

        // Act
        var result = await handler.Handle(q, default);

        // Assert: repo called exactly once with same args
        readRepo.Verify(x => x.GetPagedAsync(
            2, 10, "Name", "asc", It.IsAny<CancellationToken>()), Times.Once);

        // Assert: mapping happened for each item
        mapper.Verify(m => m.Map<ProductDto>(p1), Times.Once);
        mapper.Verify(m => m.Map<ProductDto>(p2), Times.Once);

        // Assert: QueryResult wrapper
        Assert.Equal(CachingDataSource.Db, result.Source);
        Assert.Equal(0, result.ElapsedMs);

        // Assert: paged result
        Assert.Equal(total, result.Data.Total);
        Assert.Equal(2, result.Data.PageNumber);
        Assert.Equal(10, result.Data.PageSize);
        Assert.Equal(2, result.Data.Items.Count);

        Assert.Equal(p1.Id, result.Data.Items[0].Id);
        Assert.Equal("SKU-1", result.Data.Items[0].Sku);

        readRepo.VerifyAll();
        mapper.VerifyAll();
    }

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

