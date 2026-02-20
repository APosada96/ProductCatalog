using MediatR;
using Microsoft.Extensions.Caching.Memory;


namespace ProductCatalog.Application.Events;
public sealed class CacheInvalidationHandler(IMemoryCache cache)
    : INotificationHandler<CacheInvalidationRequested>
{
    public Task Handle(CacheInvalidationRequested notification, CancellationToken ct)
    {
        cache.Remove(notification.CacheKey);
        return Task.CompletedTask;
    }
}
