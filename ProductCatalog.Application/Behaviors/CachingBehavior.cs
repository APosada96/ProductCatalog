using MediatR;
using Microsoft.Extensions.Caching.Memory;
using ProductCatalog.Application.Abstractions.Caching;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ProductCatalog.Application.Behaviors;

public sealed class CachingBehavior<TReq, TRes>(
    IMemoryCache cache,
    ConcurrentDictionary<string, SemaphoreSlim> locks)
    : IPipelineBehavior<TReq, TRes>
    where TReq : ICacheableQuery
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var isQueryResult = typeof(TRes).IsGenericType &&
                            typeof(TRes).GetGenericTypeDefinition() == typeof(QueryResult<>);

        var sw = Stopwatch.StartNew();

        if (cache.TryGetValue(request.CacheKey, out TRes? cached) && cached is not null)
        {
            sw.Stop();
            return isQueryResult ? RewrapAsCache(cached, sw.ElapsedMilliseconds) : cached;
        }

        var sem = locks.GetOrAdd(request.CacheKey, _ => new SemaphoreSlim(1, 1));
        await sem.WaitAsync(ct);
        try
        {
            if (cache.TryGetValue(request.CacheKey, out cached) && cached is not null)
            {
                sw.Stop();
                return isQueryResult ? RewrapAsCache(cached, sw.ElapsedMilliseconds) : cached;
            }

            var result = await next();

            cache.Set(request.CacheKey, result, request.Ttl);

            sw.Stop();
            return isQueryResult ? RewrapAsDb(result, sw.ElapsedMilliseconds) : result;
        }
        finally
        {
            sem.Release();
        }
    }

    private static TRes RewrapAsCache(TRes original, long elapsedMs)
    {
        var dataProp = original!.GetType().GetProperty("Data")!;
        var data = dataProp.GetValue(original);

        var ctor = original.GetType().GetConstructor(new[] { data!.GetType(), typeof(DataSource), typeof(long) })!;
        return (TRes)ctor.Invoke(new[] { data!, DataSource.Cache, elapsedMs });
    }

    private static TRes RewrapAsDb(TRes original, long elapsedMs)
    {
        var dataProp = original!.GetType().GetProperty("Data")!;
        var data = dataProp.GetValue(original);

        var ctor = original.GetType().GetConstructor(new[] { data!.GetType(), typeof(DataSource), typeof(long) })!;
        return (TRes)ctor.Invoke(new[] { data!, DataSource.Db, elapsedMs });
    }
}
