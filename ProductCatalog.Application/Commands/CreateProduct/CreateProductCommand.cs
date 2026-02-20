using MediatR;
using ProductCatalog.Application.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Application.Commands.CreateProduct;
public sealed record CreateProductCommand(
    string Name,
    string Sku,
    decimal SalePrice,
    decimal Cost,
    int Stock,
    Guid? RequestId
) : IRequest<Guid>, IIdempotentRequest;
