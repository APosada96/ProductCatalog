namespace ProductCatalog.Application.Abstractions.Idempotency;

public static class IdempotencyKey
{
    public static string Build(string requestName, Guid requestId)
        => $"{requestName}:{requestId:D}";
}
