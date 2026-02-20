namespace ProductCatalog.Api.Contracts;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
);
