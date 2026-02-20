using MediatR;
using ProductCatalog.Application.Abstractions.Caching;
using CachingQueryResult = ProductCatalog.Application.Abstractions.Caching.QueryResult<ProductCatalog.Application.DTOs.ProductDto?>;

namespace ProductCatalog.Application.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id)
    : IRequest<CachingQueryResult>, ICacheableQuery
{
    public string CacheKey => CacheKeys.ProductById(Id);
    public TimeSpan Ttl => TimeSpan.FromSeconds(30);
}