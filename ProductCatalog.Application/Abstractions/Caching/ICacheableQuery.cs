namespace ProductCatalog.Application.Abstractions.Caching;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan Ttl { get; }
}
