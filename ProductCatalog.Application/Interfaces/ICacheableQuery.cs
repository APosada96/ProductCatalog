using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCatalog.Application.Interfaces;

    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan Ttl { get; }
    }

    public enum DataSource { Cache, Db }

    public sealed record QueryResult<T>(T Data, DataSource Source, long ElapsedMs);