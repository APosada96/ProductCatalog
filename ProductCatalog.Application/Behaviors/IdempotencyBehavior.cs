using MediatR;
using ProductCatalog.Application.Abstractions.Idempotency;

namespace ProductCatalog.Application.Behaviors;

public interface IIdempotentRequest
{
    Guid? RequestId { get; }
}
public sealed class IdempotencyBehavior<TReq, TRes>(
    IIdempotencyStore store)
    : IPipelineBehavior<TReq, TRes>
    where TReq : IIdempotentRequest
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (request.RequestId is not Guid rid)
            return await next();

        var key = IdempotencyKey.Build(typeof(TReq).Name, rid);

        if (await store.HasProcessedAsync(key, ct))
            return default!; 

        var response = await next();

        await store.MarkProcessedAsync(key, ct);

        return response;
    }
}
