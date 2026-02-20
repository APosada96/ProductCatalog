using AutoMapper;
using MediatR;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

using CachingQueryResult = ProductCatalog.Application.Abstractions.Caching.QueryResult<ProductCatalog.Application.DTOs.ProductDto?>;
using CachingDataSource = ProductCatalog.Application.Abstractions.Caching.DataSource;

namespace ProductCatalog.Application.Queries.GetProductById;

public sealed class GetProductByIdHandler(
    IProductReadRepository readRepo,
    IMapper mapper)
    : IRequestHandler<GetProductByIdQuery, CachingQueryResult>
{
    public async Task<CachingQueryResult> Handle(GetProductByIdQuery q, CancellationToken ct)
    {
        var product = await readRepo.GetByIdAsync(q.Id, ct);
        var dto = product is null ? null : mapper.Map<ProductDto>(product);

        return new CachingQueryResult(dto, CachingDataSource.Db, 0);
    }
}
