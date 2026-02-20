using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Interfaces;

public interface IProductReadRepository
{
    Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        CancellationToken ct);

    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsBySkuAsync(string normalizedSku, CancellationToken ct);
}

