using AutoMapper;
using MediatR;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

using CachingQueryResult = ProductCatalog.Application.Abstractions.Caching.QueryResult<ProductCatalog.Application.DTOs.PagedResult<ProductCatalog.Application.DTOs.ProductDto>>;
using CachingDataSource = ProductCatalog.Application.Abstractions.Caching.DataSource;

namespace ProductCatalog.Application.Queries.GetProducts;

public sealed class GetProductsHandler(
    IProductReadRepository readRepo,
    IMapper mapper)
    : IRequestHandler<GetProductsQuery, CachingQueryResult>
{
    public async Task<CachingQueryResult> Handle(GetProductsQuery q, CancellationToken ct)
    {
        var (items, total) = await readRepo.GetPagedAsync(
            q.PageNumber, q.PageSize, q.SortField, q.SortDirection, ct);

        var dto = items.Select(mapper.Map<ProductDto>).ToList();
        var paged = new PagedResult<ProductDto>(dto, total, q.PageNumber, q.PageSize);

        return new CachingQueryResult(paged, CachingDataSource.Db, 0);
    }
}
