using MediatR;
using Moq;
using ProductCatalog.Application.Abstractions.Caching;
using ProductCatalog.Application.Commands.UpdateProduct;
using ProductCatalog.Application.Events;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Events;
using ProductCatalog.Domain.ValueObjects;

namespace ProductCatalog.UnitTests.Application;

public sealed class UpdateProductHandlerTests
{
    [Fact]
    public async Task When_Product_Not_Found_Should_Throw_And_Not_Persist_Or_Publish()
    {
        // Arrange
        var writeRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        readRepo
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductHandler(writeRepo.Object, readRepo.Object, mediator.Object);

        var cmd = new UpdateProductCommand(
            Id: Guid.NewGuid(),
            Name: "New",
            SalePrice: 100m,
            Cost: 50m,
            Stock: 10,
            RequestId: null);

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(cmd, default));

        writeRepo.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        writeRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);

        readRepo.VerifyAll();
        writeRepo.VerifyAll();
        mediator.VerifyAll();
    }

    [Fact]
    public async Task When_SalePrice_Less_Than_Cost_Should_Throw_And_Not_Persist_Or_Publish()
    {
        // Arrange
        var writeRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var product = new Product(
            Guid.NewGuid(),
            "Old",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            stock: 5);

        readRepo
            .Setup(x => x.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new UpdateProductHandler(writeRepo.Object, readRepo.Object, mediator.Object);

        var cmd = new UpdateProductCommand(
            Id: product.Id,
            Name: "New Name",
            SalePrice: 40m, // ❌
            Cost: 50m,
            Stock: 5,
            RequestId: null);

        // Act + Assert
        await Assert.ThrowsAsync<InvariantViolationException>(() => handler.Handle(cmd, default));

        writeRepo.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        writeRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);

        readRepo.VerifyAll();
        writeRepo.VerifyAll();
        mediator.VerifyAll();
    }

    

    [Fact]
    public async Task When_SaveChanges_Fails_Should_Not_Publish_Any_Notifications()
    {
        // Arrange
        var writeRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);
        var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
        var mediator = new Mock<IMediator>(MockBehavior.Strict);

        var product = new Product(
            Guid.NewGuid(),
            "Old",
            new Sku("ABC-1"),
            new Money(100m),
            new Money(50m),
            stock: 5);

        product.ClearEvents();

        readRepo
            .Setup(x => x.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        writeRepo
            .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        writeRepo
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("Transient failure"));

        var handler = new UpdateProductHandler(writeRepo.Object, readRepo.Object, mediator.Object);

        var cmd = new UpdateProductCommand(
            Id: product.Id,
            Name: "Updated",
            SalePrice: 120m,
            Cost: 60m,
            Stock: 10,
            RequestId: null);

        // Act + Assert
        await Assert.ThrowsAsync<TimeoutException>(() => handler.Handle(cmd, default));

        mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);

        writeRepo.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        writeRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        readRepo.VerifyAll();
        writeRepo.VerifyAll();
        mediator.VerifyAll();
    }
}
