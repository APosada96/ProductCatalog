using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.ValueObjects;
using ProductCatalog.Infrastructure.Persistence;

namespace ProductCatalog.Infrastructure.Seed;

public static class ProductSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var rnd = new Random();

        var products = Enumerable.Range(1, 20).Select(i =>
        {
            var cost = rnd.Next(10, 100);
            var sale = cost + rnd.Next(5, 50);

            return new Product(
                Guid.NewGuid(),
                $"Product {i}",
                new Sku($"PRD-{i:000}"),
                new Money(sale),
                new Money(cost),
                rnd.Next(0, 100));
        });

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
