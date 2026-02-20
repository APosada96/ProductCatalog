using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure.Repositories;

public sealed class ProductWriteRepository : IProductWriteRepository
{
    private readonly AppDbContext _context;

    public ProductWriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Product product, CancellationToken ct)
        => _context.Products.AddAsync(product, ct).AsTask();

    public Task UpdateAsync(Product product, CancellationToken ct)
    {
        _context.Products.Update(product);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken ct)
    {
        _context.Products.Remove(product);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
        => _context.SaveChangesAsync(ct);
}
