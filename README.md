# 📦 Product Catalog -- Prueba Técnica (.NET 10)

Prueba de concepto full-stack implementando DDD estricto, CQRS, eventos
de dominio, behaviors transversales, EF Core avanzado, Minimal APIs y
frontend en Blazor WebAssembly.

------------------------------------------------------------------------

# 🏆 Objetivo

-   Modelado de dominio real
-   Separación estricta por capas
-   Implementación correcta de CQRS
-   Eventos de dominio desacoplados
-   Pipeline behaviors avanzados
-   Configuración rigurosa de EF Core
-   Validación robusta en UI
-   Testing de dominio y aplicación

------------------------------------------------------------------------

# 🧠 Arquitectura General

``` mermaid
flowchart TD
    UI[Blazor WASM]
    API[Minimal API]
    APP[Application Layer]
    DOMAIN[Domain Layer]
    INFRA[Infrastructure Layer]
    DB[(SQL Server)]

    UI --> API
    API --> APP
    APP --> DOMAIN
    APP --> INFRA
    INFRA --> DB
```

------------------------------------------------------------------------

# 📦 Estructura del Proyecto

ProductCatalog.Domain\
ProductCatalog.Application\
ProductCatalog.Infrastructure\
ProductCatalog.Api\
ProductCatalog.Blazor\
ProductCatalog.UnitTests

------------------------------------------------------------------------

# 🧱 Dominio

Entidad principal: Product

Invariantes: - Name no vacío - SKU obligatorio - SalePrice ≥ Cost -
Stock ≥ 0

Uso de Value Objects: - Money - Sku

Eventos de dominio: - ProductCreated - ProductChanged

------------------------------------------------------------------------

# 🚦 Application

Implementación de CQRS con:

Commands: - CreateProduct - UpdateProduct - DeleteProduct

Queries: - GetProducts - GetProductById

Pipeline Behaviors: - Validation - Logging - Caching (anti stampede) -
Idempotency

------------------------------------------------------------------------

# 🗄 Infrastructure

EF Core configurado con: - Índice único en SKU - ValueConverters para
Money y Sku - Precision decimal 18,2 - Retry automático - Interceptor de
queries lentas

------------------------------------------------------------------------

# 🖥 Blazor WASM

-   Validación por campo
-   Validación asíncrona de SKU
-   Indicador Cache / DB
-   Manejo unificado de errores

------------------------------------------------------------------------

# 🧪 Testing

-   Validación de invariantes
-   Publicación de eventos
-   Pruebas de handlers
-   Validación de AutoMapper

------------------------------------------------------------------------

# 🚀 Cómo Ejecutar

1)  Configurar connection string en appsettings.json\
2)  Ejecutar migraciones EF Core\
3)  dotnet run en Api\
4)  dotnet run en Blazor

------------------------------------------------------------------------

Desarrollado como ejercicio técnico demostrando buenas prácticas de
arquitectura empresarial en .NET.
