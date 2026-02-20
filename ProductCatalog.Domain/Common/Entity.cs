namespace ProductCatalog.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new InvariantViolationException("Id must not be empty.");

        Id = id;
    }

    protected Entity() { }

    public override bool Equals(object? obj)
        => obj is Entity other && Id == other.Id && GetType() == other.GetType();

    public override int GetHashCode()
        => HashCode.Combine(GetType(), Id);
}
