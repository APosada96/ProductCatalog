using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Contracts;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Application.Queries.GetProductById;
using ProductCatalog.Application.Queries.GetProducts;
using ProductCatalog.Domain.ValueObjects;

using ProductCatalog.Application.Commands.CreateProduct;
using ProductCatalog.Application.Commands.UpdateProduct;
using ProductCatalog.Application.Commands.DeleteProduct;

namespace ProductCatalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", GetProducts);
        group.MapGet("/{id:guid}", GetProductById);

        group.MapGet("/sku-exists/{sku}", SkuExists);

        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:guid}", UpdateProduct);
        group.MapDelete("/{id:guid}", DeleteProduct);

        return app;
    }

    private static async Task<IResult> GetProducts(
        IMediator mediator,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortField = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken ct = default)
    {
        var q = new GetProductsQuery(pageNumber, pageSize, sortField, sortDirection);
        var result = await mediator.Send(q, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(
        IMediator mediator,
        Guid id,
        CancellationToken ct = default)
    {
        var q = new GetProductByIdQuery(id);
        var result = await mediator.Send(q, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> SkuExists(
        IProductReadRepository readRepo,
        string sku,
        CancellationToken ct = default)
    {
        var normalized = Sku.Normalize(sku);
        var exists = await readRepo.ExistsBySkuAsync(normalized, ct);
        return Results.Ok(new { sku = normalized, exists });
    }

    private static async Task<IResult> CreateProduct(
        IMediator mediator,
        CreateProductRequest request,
        CancellationToken ct = default)
    {
        var cmd = new CreateProductCommand(
            request.Name,
            request.Sku,
            request.SalePrice,
            request.Cost,
            request.Stock,
            request.RequestId);

        var id = await mediator.Send(cmd, ct);
        return Results.Created($"/api/products/{id}", new { id });
    }

    private static async Task<IResult> UpdateProduct(
        IMediator mediator,
        Guid id,
        UpdateProductRequest request,
        CancellationToken ct = default)
    {
        var cmd = new UpdateProductCommand(
            id,
            request.Name,
            request.SalePrice,
            request.Cost,
            request.Stock,
            request.RequestId);

        await mediator.Send(cmd, ct);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteProduct(
        IMediator mediator,
        Guid id,
        CancellationToken ct = default)
    {
        await mediator.Send(new DeleteProductCommand(id), ct);
        return Results.NoContent();
    }
}
