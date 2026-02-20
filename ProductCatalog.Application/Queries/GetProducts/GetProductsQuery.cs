using MediatR;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.DTOs;

using CachingQueryResult = ProductCatalog.Application.Abstractions.Caching.QueryResult<ProductCatalog.Application.DTOs.PagedResult<ProductCatalog.Application.DTOs.ProductDto>>;

namespace ProductCatalog.Application.Queries.GetProducts;

public sealed record GetProductsQuery(
    int PageNumber,
    int PageSize,
    string? SortField,
    string? SortDirection
) : IRequest<CachingQueryResult>, ICacheableQuery
{
    public string CacheKey => CacheKeys.ProductsList(PageNumber, PageSize, SortField, SortDirection);
    public TimeSpan Ttl => TimeSpan.FromSeconds(30);
}