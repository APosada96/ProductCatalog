namespace ProductCatalog.Application.DTOs;

public sealed record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Sku { get; init; } = "";
    public decimal SalePrice { get; init; }
    public decimal Cost { get; init; }
    public int Stock { get; init; }
    public decimal MarginPercent { get; init; }
}
