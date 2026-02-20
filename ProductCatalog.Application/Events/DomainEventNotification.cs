using MediatR;
using ProductCatalog.Domain.Events;

namespace ProductCatalog.Application.Events;
public sealed record DomainEventNotification<TEvent>(TEvent DomainEvent) : INotification
    where TEvent : IDomainEvent;
