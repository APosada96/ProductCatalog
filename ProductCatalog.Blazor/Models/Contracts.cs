namespace ProductCatalog.Blazor.Models;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
);

public sealed record UpdateProductRequest(
    string Name,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
);

public sealed record CreateProductResponse(Guid Id);

public sealed record SkuExistsResponse(string Sku, bool Exists);
