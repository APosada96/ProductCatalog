using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Common;

namespace ProductCatalog.Api.Middleware;

public static class ExceptionHandlingExtensions
{
    public static WebApplication UseApiExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                if (ex is null)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Unexpected error"
                    });
                    return;
                }

                var isDev = app.Environment.IsDevelopment();

                var (status, title, detail, errors) = MapException(ex, isDev);

                context.Response.StatusCode = status;

                var pd = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail
                };

                if (errors is not null)
                    pd.Extensions["errors"] = errors;

                await context.Response.WriteAsJsonAsync(pd);
            });
        });

        return app;
    }

    private static (int Status, string Title, string Detail, object? Errors) MapException(Exception ex, bool isDev)
    {
        switch (ex)
        {
            case ValidationException vex:
                {
                    var errors = vex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                    return (StatusCodes.Status400BadRequest,
                            "Validation failed",
                            "Please correct the highlighted fields.",
                            errors);
                }

            case InvariantViolationException inv:
                return (StatusCodes.Status400BadRequest,
                        "Business rule violation",
                        inv.Message,
                        null);

            case DomainException de:
                return (StatusCodes.Status400BadRequest,
                        "Domain error",
                        de.Message,
                        null);

            case DbUpdateException dbex when LooksLikeUniqueViolation(dbex):
                return (StatusCodes.Status409Conflict,
                        "Conflict",
                        isDev ? dbex.ToString() : "A resource with the same unique key already exists.",
                        null);

            default:
                return (StatusCodes.Status500InternalServerError,
                        "Server error",
                        isDev ? ex.ToString() : "An unexpected error occurred.",
                        null);
        }
    }

    private static bool LooksLikeUniqueViolation(DbUpdateException ex)
    {
        // SQL Server: 2601 (duplicate key row), 2627 (unique constraint)
        var msg = ex.InnerException?.Message ?? ex.Message;
        return msg.Contains("2601") || msg.Contains("2627") ||
               msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
    }
}
