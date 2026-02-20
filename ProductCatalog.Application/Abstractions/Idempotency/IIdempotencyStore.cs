namespace ProductCatalog.Application.Abstractions.Idempotency;

public interface IIdempotencyStore
{
    Task<bool> HasProcessedAsync(string key, CancellationToken ct);
    Task MarkProcessedAsync(string key, CancellationToken ct);
}


