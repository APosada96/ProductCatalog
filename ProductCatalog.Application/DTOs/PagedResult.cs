
namespace ProductCatalog.Application.DTOs;
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Total,
    int PageNumber,
    int PageSize
);
