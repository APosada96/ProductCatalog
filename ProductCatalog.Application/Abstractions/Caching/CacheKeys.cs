namespace ProductCatalog.Application.Abstractions.Caching;

public static class CacheKeys
{
    public static string ProductsList(
        int pageNumber, int pageSize, string? sortField, string? sortDirection)
        => $"products:list:p{pageNumber}:s{pageSize}:sf{sortField ?? "null"}:sd{sortDirection ?? "null"}";

    public static string ProductById(Guid id) => $"products:id:{id:D}";
}
