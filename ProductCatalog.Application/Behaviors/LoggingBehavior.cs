using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ProductCatalog.Application.Behaviors;

public sealed class LoggingBehavior<TReq, TRes>(
    ILogger<LoggingBehavior<TReq, TRes>> logger)
    : IPipelineBehavior<TReq, TRes>
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        logger.LogInformation("➡️ {RequestType}: {@Request}", typeof(TReq).Name, request);

        var response = await next();

        sw.Stop();
        logger.LogInformation("✅ {RequestType} handled in {Elapsed}ms", typeof(TReq).Name, sw.ElapsedMilliseconds);

        return response;
    }
}
