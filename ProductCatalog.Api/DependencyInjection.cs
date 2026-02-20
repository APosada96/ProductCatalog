using FluentValidation;
using MediatR;
using ProductCatalog.Application.Behaviors;
using ProductCatalog.Application.Mapping;
using System.Collections.Concurrent;

namespace ProductCatalog.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<MappingAssemblyMarker>());

        services.AddAutoMapper(typeof(MappingAssemblyMarker));
        services.AddAutoMapper(typeof(ProductProfile).Assembly);

        services.AddValidatorsFromAssemblyContaining<MappingAssemblyMarker>();

        services.AddMemoryCache();
        services.AddSingleton(new ConcurrentDictionary<string, SemaphoreSlim>());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));

        return services;
    }
}
