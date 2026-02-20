using MediatR;
using ProductCatalog.Application.Events;
using ProductCatalog.Domain.Events;

public sealed class ProductCreatedNotificationHandler
    : INotificationHandler<DomainEventNotification<ProductCreated>>
{
    public Task Handle(DomainEventNotification<ProductCreated> notification, CancellationToken ct)
    {
        var evt = notification.DomainEvent;
        return Task.CompletedTask;
    }
}