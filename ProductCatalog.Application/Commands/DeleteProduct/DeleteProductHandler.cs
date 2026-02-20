using MediatR;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Events;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;


namespace ProductCatalog.Application.Commands.DeleteProduct;
public sealed class DeleteProductHandler(
    IProductReadRepository readRepo,
    IProductWriteRepository writeRepo,
    IMediator mediator)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand cmd, CancellationToken ct)
    {
        var product = await readRepo.GetByIdAsync(cmd.Id, ct);
        if (product is null)
            throw new DomainException("Product not found.");

        await writeRepo.DeleteAsync(product, ct);
        await writeRepo.SaveChangesAsync(ct);

        await mediator.Publish(new CacheInvalidationRequested(CacheKeys.ProductById(cmd.Id)), ct);
        await mediator.Publish(new CacheInvalidationRequested("products:list"), ct);
    }
}
