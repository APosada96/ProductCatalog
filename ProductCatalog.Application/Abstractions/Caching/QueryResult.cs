namespace ProductCatalog.Application.Abstractions.Caching;

public enum DataSource
{
    Cache = 1,
    Db = 2
}

public sealed record QueryResult<T>(T Data, DataSource Source, long ElapsedMs);
