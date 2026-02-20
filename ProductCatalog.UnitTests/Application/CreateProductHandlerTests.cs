using MediatR;
using Moq;
using ProductCatalog.Application.Commands.CreateProduct;
using ProductCatalog.Application.Events;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Domain.Common;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Events;

namespace ProductCatalog.UnitTests.Application;

    public sealed class CreateProductHandlerTests
    {
        [Fact]
        public async Task When_Sku_Already_Exists_Should_Throw_And_Not_Persist_Or_Publish()
        {
            var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
            var writeRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            readRepo
                .Setup(x => x.ExistsBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var handler = new CreateProductHandler(readRepo.Object, writeRepo.Object, mediator.Object);

            var cmd = new CreateProductCommand(
                Name: "Test",
                Sku: " ab-0o1 ",
                SalePrice: 100m,
                Cost: 50m,
                Stock: 10,
                RequestId: null);

            await Assert.ThrowsAsync<InvariantViolationException>(() => handler.Handle(cmd, default));

            writeRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
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
            var readRepo = new Mock<IProductReadRepository>(MockBehavior.Strict);
            var writeRepo = new Mock<IProductWriteRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            readRepo
                .Setup(x => x.ExistsBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            writeRepo
                .Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            writeRepo
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TimeoutException("Transient failure"));

            var handler = new CreateProductHandler(readRepo.Object, writeRepo.Object, mediator.Object);

            var cmd = new CreateProductCommand(
                Name: "Test",
                Sku: "ABC-1",
                SalePrice: 100m,
                Cost: 50m,
                Stock: 10,
                RequestId: null);

            await Assert.ThrowsAsync<TimeoutException>(() => handler.Handle(cmd, default));

            mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Never);

            writeRepo.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            writeRepo.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            readRepo.VerifyAll();
            writeRepo.VerifyAll();
            mediator.VerifyAll();
        }
    }
