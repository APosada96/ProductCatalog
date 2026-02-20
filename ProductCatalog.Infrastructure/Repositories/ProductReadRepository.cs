using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure.Repositories;

public sealed class ProductReadRepository : IProductReadRepository
{
    private readonly AppDbContext _context;

    public ProductReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<Product> Items, int Total)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? sortField,
        string? sortDirection,
        CancellationToken ct)
    {
        var query = _context.Products.AsNoTracking();

        // Orden dinámico simple
        query = (sortField?.ToLower(), sortDirection?.ToLower()) switch
        {
            ("name", "desc") => query.OrderByDescending(x => x.Name),
            ("name", _) => query.OrderBy(x => x.Name),
            ("price", "desc") => query.OrderByDescending(x => x.SalePrice.Amount),
            ("price", _) => query.OrderBy(x => x.SalePrice.Amount),
            _ => query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<bool> ExistsBySkuAsync(string normalizedSku, CancellationToken ct)
    {
        var sku = new Sku(normalizedSku); 
        return _context.Products
            .AsNoTracking()
            .AnyAsync(x => x.Sku == sku, ct);
    }
}
