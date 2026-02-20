using ProductCatalog.Domain.Common;

namespace ProductCatalog.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new InvariantViolationException("Money amount cannot be negative.");

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static Money Zero => new(0);

    public Money Add(Money other) => new(Amount + other.Amount);
    public Money Subtract(Money other)
    {
        var result = Amount - other.Amount;
        if (result < 0)
            throw new InvariantViolationException("Money operation would result in negative amount.");

        return new Money(result);
    }

    public override string ToString() => Amount.ToString("0.00");
}
