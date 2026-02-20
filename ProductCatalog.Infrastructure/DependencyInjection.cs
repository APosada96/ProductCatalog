using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Abstractions.Idempotency;
using ProductCatalog.Application.Interfaces;
using ProductCatalog.Infrastructure.Idempotency;
using ProductCatalog.Infrastructure.Persistence;
using ProductCatalog.Infrastructure.Persistence.Interceptors;
using ProductCatalog.Infrastructure.Repositories;

namespace ProductCatalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseSqlServer(
                configuration.GetConnectionString("Default"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });

            opt.AddInterceptors(sp.GetRequiredService<SlowQueryLoggingInterceptor>());
            opt.EnableDetailedErrors();
        });

        services.AddScoped<IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
        services.AddScoped<IIdempotencyStore, EfIdempotencyStore>();
        services.AddScoped<SlowQueryLoggingInterceptor>();
        return services;
    }
}
