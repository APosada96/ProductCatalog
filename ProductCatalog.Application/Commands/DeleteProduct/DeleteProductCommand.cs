using MediatR;


namespace ProductCatalog.Application.Commands.DeleteProduct;
public sealed record DeleteProductCommand(Guid Id) : IRequest;

