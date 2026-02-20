namespace ProductCatalog.Domain.Events;

public sealed record ProductChanged(Guid ProductId, DateTimeOffset OccurredAt) : IDomainEvent;
