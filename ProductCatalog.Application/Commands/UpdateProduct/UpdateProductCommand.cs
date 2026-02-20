using MediatR;
using ProductCatalog.Application.Behaviors;


namespace ProductCatalog.Application.Commands.UpdateProduct;
public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
) : IRequest, IIdempotentRequest;
