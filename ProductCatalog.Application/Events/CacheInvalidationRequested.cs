using MediatR;

namespace ProductCatalog.Application.Events;
public sealed record CacheInvalidationRequested(string CacheKey) : INotification;

