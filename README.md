Product Catalog – Prueba Técnica (.NET 10)

Prueba de concepto full-stack implementando DDD estricto, CQRS, eventos de dominio en proceso, behaviors transversales, EF Core avanzado, API con Minimal APIs y frontend en Blazor WebAssembly.

Objetivo de la Prueba

Demostrar:

Modelado de dominio real (no CRUD anémico)

Separación estricta de responsabilidades

Uso correcto de CQRS

Eventos de dominio desacoplados

Pipeline behaviors avanzados

Configuración avanzada de EF Core

UI con validación robusta

Testing de dominio y aplicación

Buenas prácticas de arquitectura empresarial

Arquitectura General

El proyecto sigue Clean Architecture con separación clara por capas.

Diagrama de Capas

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

Estructura del Proyecto
ProductCatalog.Domain
ProductCatalog.Application
ProductCatalog.Infrastructure
ProductCatalog.Api
ProductCatalog.Blazor
ProductCatalog.UnitTests

Capa de Dominio

Contiene únicamente lógica de negocio pura.

Entidad Principal: Product
Invariantes

Name no puede ser vacío

SKU no puede ser vacío

SalePrice ≥ Cost

Stock ≥ 0

Seguridad ante fallos

Cada mutación se ejecuta dentro de:

ApplySafely(Action<Product>)

Si alguna regla falla:

Se restaura el estado anterior (snapshot)

Se lanza excepción de dominio

Value Objects
Money

No permite valores negativos

Igualdad estructural

Controla precisión decimal

Sku

Normalización avanzada

Evita inconsistencias de formato

Igualdad estructural

Eventos de Dominio

ProductCreated

ProductChanged

El Dominio NO depende de MediatR.
La capa Application se encarga de adaptarlos.

Capa Application

Implementa CQRS y comportamientos transversales.

Diagrama CQRS

flowchart LR
    Controller --> Command
    Controller --> Query
    Command --> Handler
    Query --> Handler
    Handler --> Repository

Commands:

CreateProduct

UpdateProduct

DeleteProduct

Queries:

GetProducts

GetProductById

Separación estricta lectura/escritura.

🔄 Pipeline Behaviors

flowchart LR
    Request --> ValidationBehavior
    ValidationBehavior --> LoggingBehavior
    LoggingBehavior --> CachingBehavior
    CachingBehavior --> IdempotencyBehavior
    IdempotencyBehavior --> Handler

✔ ValidationBehavior

FluentValidation

Errores centralizados

✔ LoggingBehavior

Tiempo de ejecución

Logging estructurado

✔ CachingBehavior

IMemoryCache

Anti cache stampede con SemaphoreSlim

Indica fuente: Cache / DB

Devuelve tiempo en ms

✔ IdempotencyBehavior

Previene ejecución duplicada de comandos

Persistido en base de datos

🗄 Capa Infrastructure
EF Core

Índice único en SKU

ValueConverters para:

Money

Sku

Precision decimal (18,2)

Retry automático en fallos transitorios

Interceptor de queries lentas

Repositorios
IProductReadRepository

Paginación

Orden dinámico

AsNoTracking

IProductWriteRepository

Add

Update

Delete

SaveChanges

Frontend – Blazor WASM:
Funcionalidades:
✔ Formularios con validación robusta

Validación por campo

Validación asíncrona del SKU

Botón deshabilitado si es inválido

Manejo de errores globales

✔ Indicador de fuente de datos

La lista muestra:

Fuente: Cache / DB

Tiempo de respuesta en ms

✔ Manejo unificado de errores

ErrorBoundary personalizado

Modo Development muestra detalle técnico

Producción muestra mensaje amigable

Testing:
Dominio

Validación de invariantes

Verificación de rollback

Eventos generados correctamente

Application

Detección de SKU duplicado

Publicación de eventos

Invalidación de cache

Mapping

Validación de AutoMapper

DTO contract testing

Cómo Ejecutar el Proyecto:
Requisitos

.NET 10 SDK

SQL Server o SQL Express

1️⃣ Configurar cadena de conexión

En ProductCatalog.Api/appsettings.json:

"ConnectionStrings": {
  "Default": "Server=localhost;Database=ProductCatalogDb;Trusted_Connection=True;TrustServerCertificate=True"
}
2️⃣ Ejecutar migraciones
dotnet ef migrations add InitialCreate -p ProductCatalog.Infrastructure -s ProductCatalog.Api
dotnet ef database update -p ProductCatalog.Infrastructure -s ProductCatalog.Api
3️⃣ Ejecutar API
dotnet run --project ProductCatalog.Api

Swagger disponible en:

https://localhost:<puerto>/swagger
4️⃣ Ejecutar Blazor
dotnet run --project ProductCatalog.Blazor

Autor:

Desarrollado como ejercicio técnico demostrando buenas prácticas de arquitectura empresarial en .NET.
