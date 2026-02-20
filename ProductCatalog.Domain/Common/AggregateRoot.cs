using ProductCatalog.Domain.Events;

namespace ProductCatalog.Domain.Common;

public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _events = new();

    public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

    protected AggregateRoot(Guid id) : base(id) { }
    protected AggregateRoot() { }

    protected void AddEvent(IDomainEvent domainEvent)
    {
       
        var type = domainEvent.GetType();

        var lastIdx = _events.FindLastIndex(e => e.GetType() == type);
        if (lastIdx >= 0)
        {
            _events[lastIdx] = domainEvent;
            return;
        }

        _events.Add(domainEvent);
    }

    public void ClearEvents() => _events.Clear();
}
