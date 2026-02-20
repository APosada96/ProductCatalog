namespace ProductCatalog.Api.Contracts;

public sealed record UpdateProductRequest(
    string Name,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
);
