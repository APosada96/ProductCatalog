using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.Domain.Entities;

public sealed class Product : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public Sku Sku { get; private set; } = default!;
    public Money SalePrice { get; private set; } = Money.Zero;
    public Money Cost { get; private set; } = Money.Zero;
    public int Stock { get; private set; }
    public bool PriceLocked { get; private set; }

    private Product() { }

    public Product(
        Guid id,
        string name,
        Sku sku,
        Money salePrice,
        Money cost,
        int stock) : base(id)
    {
        ApplySafely(p =>
        {
            p.SetName(name);
            p.SetSku(sku);
            p.SetPrices(salePrice, cost);
            p.SetStock(stock);

            p.AddEvent(new ProductCreated(p.Id, DateTimeOffset.UtcNow));
        });
    }

    public void UpdateName(string name)
        => ApplySafely(p =>
        {
            p.SetName(name);
            p.AddChanged();
        });

    public void UpdatePrice(Money salePrice, Money cost)
        => ApplySafely(p =>
        {
            if (p.PriceLocked)
                throw new InvariantViolationException("Price is locked and cannot be modified.");

            p.SetPrices(salePrice, cost);
            p.AddChanged();
        });

    public void AdjustStock(int delta)
        => ApplySafely(p =>
        {
            checked
            {
                var newStock = p.Stock + delta;
                if (newStock < 0)
                    throw new InvariantViolationException("Stock cannot be negative.");

                p.Stock = newStock;
            }

            p.AddChanged();
        });

    public void LockPrice()
        => ApplySafely(p =>
        {
            p.PriceLocked = true;
            p.AddChanged();
        });

    public void UnlockPrice()
        => ApplySafely(p =>
        {
            p.PriceLocked = false;
            p.AddChanged();
        });

    public void ApplySafely(Action<Product> action)
    {
        var snapshot = Snapshot.Capture(this);

        try
        {
            action(this);
            ValidateInvariants();
        }
        catch
        {
            snapshot.Restore(this);
            throw;
        }
    }

    private void ValidateInvariants()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvariantViolationException("Name cannot be empty.");

        if (Sku is null || string.IsNullOrWhiteSpace(Sku.Value))
            throw new InvariantViolationException("SKU cannot be empty.");

        if (SalePrice.Amount < Cost.Amount)
            throw new InvariantViolationException("SalePrice must be greater than or equal to Cost.");

        if (Stock < 0)
            throw new InvariantViolationException("Stock cannot be negative.");
    }

    // ---- Internal setters (keep invariants centralized) ----
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvariantViolationException("Name cannot be empty.");

        Name = name.Trim();
    }

    private void SetSku(Sku sku)
    {
        if (sku is null || string.IsNullOrWhiteSpace(sku.Value))
            throw new InvariantViolationException("SKU cannot be empty.");

        Sku = sku;
    }

    private void SetPrices(Money salePrice, Money cost)
    {
        if (salePrice.Amount < cost.Amount)
            throw new InvariantViolationException("SalePrice must be greater than or equal to Cost.");

        SalePrice = salePrice;
        Cost = cost;
    }

    private void SetStock(int stock)
    {
        if (stock < 0)
            throw new InvariantViolationException("Stock cannot be negative.");

        Stock = stock;
    }

    private void AddChanged()
        => AddEvent(new ProductChanged(Id, DateTimeOffset.UtcNow));

    private sealed class Snapshot
    {
        private readonly string _name;
        private readonly Sku _sku;
        private readonly Money _salePrice;
        private readonly Money _cost;
        private readonly int _stock;
        private readonly bool _priceLocked;

        private Snapshot(string name, Sku sku, Money salePrice, Money cost, int stock, bool priceLocked)
        {
            _name = name;
            _sku = sku;
            _salePrice = salePrice;
            _cost = cost;
            _stock = stock;
            _priceLocked = priceLocked;
        }

        public static Snapshot Capture(Product p)
            => new(p.Name, p.Sku, p.SalePrice, p.Cost, p.Stock, p.PriceLocked);

        public void Restore(Product p)
        {
            p.Name = _name;
            p.Sku = _sku;
            p.SalePrice = _salePrice;
            p.Cost = _cost;
            p.Stock = _stock;
            p.PriceLocked = _priceLocked;
        }
    }
}
