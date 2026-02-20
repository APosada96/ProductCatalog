using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProductCatalog.Infrastructure.Persistence.Interceptors;

public sealed class SlowQueryLoggingInterceptor : DbCommandInterceptor
{
    private readonly ILogger<SlowQueryLoggingInterceptor> _logger;
    private readonly int _thresholdMs;

    public SlowQueryLoggingInterceptor(
        ILogger<SlowQueryLoggingInterceptor> logger,
        int thresholdMs = 200)
    {
        _logger = logger;
        _thresholdMs = thresholdMs;
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command.CommandText, eventData.Duration);

        // Puedes devolver result directo (suficiente)
        return result;

        // Si prefieres llamar base, sería:
        // return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command.CommandText, eventData.Duration);

        // Opción simple:
        return ValueTask.FromResult(result);

        // Si prefieres base:
        // return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void LogIfSlow(string sql, TimeSpan duration)
    {
        var ms = duration.TotalMilliseconds;
        if (ms >= _thresholdMs)
        {
            _logger.LogWarning(
                "Slow query detected ({Duration}ms): {Sql}",
                ms,
                sql.Length > 500 ? sql[..500] + "..." : sql);
        }
    }
}
