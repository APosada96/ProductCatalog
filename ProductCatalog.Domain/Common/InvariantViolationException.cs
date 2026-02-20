namespace ProductCatalog.Domain.Common;

public sealed class InvariantViolationException : DomainException
{
    public InvariantViolationException(string message) : base(message) { }
}
