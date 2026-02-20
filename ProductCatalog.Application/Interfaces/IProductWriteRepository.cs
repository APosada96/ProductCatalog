using ProductCatalog.Domain.Entities;


namespace ProductCatalog.Application.Interfaces;

public interface IProductWriteRepository
{
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Product product, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

