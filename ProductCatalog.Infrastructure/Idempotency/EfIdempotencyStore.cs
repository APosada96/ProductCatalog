using Microsoft.EntityFrameworkCore;
using ProductCatalog.Application.Abstractions.Idempotency;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure.Idempotency;

public sealed class EfIdempotencyStore : IIdempotencyStore
{
    private readonly AppDbContext _context;

    public EfIdempotencyStore(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasProcessedAsync(string key, CancellationToken ct)
    {
        return await _context.ProcessedRequests
            .AsNoTracking()
            .AnyAsync(x => x.Key == key, ct);
    }

    public async Task MarkProcessedAsync(string key, CancellationToken ct)
    {
        _context.ProcessedRequests.Add(new ProcessedRequest
        {
            Id = Guid.NewGuid(),
            Key = key,
            ProcessedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(ct);
    }
}

