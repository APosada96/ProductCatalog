using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProductCatalog.Application.Behaviors;
using FluentValidation;
using MediatR;

public sealed class ValidationBehavior<TReq, TRes>(
    IEnumerable<IValidator<TReq>> validators)
    : IPipelineBehavior<TReq, TRes>
{
    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var ctx = new ValidationContext<TReq>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(ctx, ct)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(e => e is not null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}

