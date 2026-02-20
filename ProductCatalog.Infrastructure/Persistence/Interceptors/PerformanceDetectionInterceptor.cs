using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProductCatalog.Infrastructure.Persistence.Interceptors;

public sealed class PerformanceDetectionInterceptor : DbCommandInterceptor
{
    private readonly ILogger<PerformanceDetectionInterceptor> _logger;
    private readonly int _slowQueryMs;

    private static readonly AsyncLocal<RequestQueryTracker> Tracker = new();

    public PerformanceDetectionInterceptor(
        ILogger<PerformanceDetectionInterceptor> logger,
        int slowQueryMs = 200)
    {
        _logger = logger;
        _slowQueryMs = slowQueryMs;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Start(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Start(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        StopAndAnalyze(command, eventData.Duration);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        StopAndAnalyze(command, eventData.Duration);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void Start(DbCommand command)
    {
        var t = Tracker.Value ??= new RequestQueryTracker();
        t.MarkQuery(command.CommandText);
    }

    private void StopAndAnalyze(DbCommand command, TimeSpan duration)
    {
        // Slow query
        var ms = (int)duration.TotalMilliseconds;
        if (ms >= _slowQueryMs)
        {
            _logger.LogWarning("🐢 Slow SQL ({Ms}ms): {Sql}", ms, Shorten(command.CommandText));
        }

        // Possible N+1 heuristic
        var t = Tracker.Value;
        if (t is null) return;

        // Si hay muchas queries similares en una ventana corta, sospecha N+1
        var signature = NormalizeSignature(command.CommandText);

        var count = t.IncrementSignature(signature);
        if (count == 10) // umbral (ajusta si quieres)
        {
            _logger.LogWarning(
                "⚠️ Possible N+1 detected: {Count} similar queries executed. Signature={Signature}",
                count,
                signature);
        }
    }

    private static string Shorten(string sql)
        => sql.Length <= 400 ? sql : sql[..400] + " ...";

    // Normaliza el SQL para que WHERE con diferentes parámetros cuente como “similar”
    private static string NormalizeSignature(string sql)
    {
        var s = sql;

        // quita saltos/espacios
        s = Regex.Replace(s, @"\s+", " ").Trim();

        // reemplaza literales numéricos y strings por '?'
        s = Regex.Replace(s, @"'[^']*'", "?");
        s = Regex.Replace(s, @"\b\d+\b", "?");

        // recorta para evitar claves enormes
        return s.Length <= 250 ? s : s[..250];
    }

    private sealed class RequestQueryTracker
    {
        private readonly ConcurrentDictionary<string, int> _signatureCounts = new();

        public void MarkQuery(string sql)
        {
            // hook por si quieres métricas totales por request
        }

        public int IncrementSignature(string signature)
        {
            return _signatureCounts.AddOrUpdate(signature, 1, (_, old) => old + 1);
        }
    }
}
