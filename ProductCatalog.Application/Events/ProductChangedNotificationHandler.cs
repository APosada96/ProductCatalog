using MediatR;
using ProductCatalog.Domain.Events;


namespace ProductCatalog.Application.Events;

public sealed class ProductChangedNotificationHandler
    : INotificationHandler<DomainEventNotification<ProductChanged>>
{
    public Task Handle(DomainEventNotification<ProductChanged> notification, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}