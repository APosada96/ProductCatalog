namespace ProductCatalog.Domain.Events;

public sealed record ProductCreated(Guid ProductId, DateTimeOffset OccurredAt) : IDomainEvent;
