using MediatR;
using ProductCatalog.Domain.Events;


namespace ProductCatalog.Application.Events;
public interface IDomainEventsPublisher
{
    Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}

public sealed class DomainEventsPublisher(IMediator mediator) : IDomainEventsPublisher
{
    public Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
        => Task.WhenAll(events.Select(e => mediator.Publish(Wrap(e), ct)));

    private static INotification Wrap(IDomainEvent e)
    {
        var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(e.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, e)!;
    }
}
