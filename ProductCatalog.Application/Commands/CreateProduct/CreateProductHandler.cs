using MediatR;
using ProductCatalog.Application.Events;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.ValueObjects;


namespace ProductCatalog.Application.Commands.CreateProduct;

public sealed class CreateProductHandler(
    IProductReadRepository readRepo,
    IProductWriteRepository writeRepo,
    IMediator mediator)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var sku = new Sku(cmd.Sku);

        if (await readRepo.ExistsBySkuAsync(sku.Value, ct))
            throw new InvariantViolationException("SKU already exists.");

        var product = new Product(
            Guid.NewGuid(),
            cmd.Name,
            sku,
            new Money(cmd.SalePrice),
            new Money(cmd.Cost),
            cmd.Stock);

        await writeRepo.AddAsync(product, ct);
        await writeRepo.SaveChangesAsync(ct);

        foreach (var e in product.Events)
            await mediator.Publish(WrapDomainEvent(e), ct);

        product.ClearEvents();

        await mediator.Publish(new CacheInvalidationRequested("products:list"), ct);

        return product.Id;
    }

    private static INotification WrapDomainEvent(IDomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        return (INotification)Activator.CreateInstance(wrapperType, domainEvent)!;
    }
}
