using MediatR;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Events;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.ValueObjects;


namespace ProductCatalog.Application.Commands.UpdateProduct;

public sealed class UpdateProductHandler(
    IProductWriteRepository writeRepo,
    IProductReadRepository readRepo,
    IMediator mediator)
    : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand cmd, CancellationToken ct)
    {
        var product = await readRepo.GetByIdAsync(cmd.Id, ct);
        if (product is null)
            throw new DomainException("Product not found.");

        product.UpdateName(cmd.Name);
        product.UpdatePrice(new Money(cmd.SalePrice), new Money(cmd.Cost));

        var delta = cmd.Stock - product.Stock;
        product.AdjustStock(delta);

        await writeRepo.UpdateAsync(product, ct);
        await writeRepo.SaveChangesAsync(ct);

        foreach (var e in product.Events)
            await mediator.Publish(WrapDomainEvent(e), ct);

        product.ClearEvents();

        await mediator.Publish(new CacheInvalidationRequested(CacheKeys.ProductById(cmd.Id)), ct);
        await mediator.Publish(new CacheInvalidationRequested("products:list"), ct);
    }

    private static INotification WrapDomainEvent(IDomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
    }
}
