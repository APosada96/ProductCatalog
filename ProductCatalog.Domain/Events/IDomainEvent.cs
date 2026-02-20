namespace ProductCatalog.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
