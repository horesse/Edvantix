using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Constants.Other;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Queries;

/// <summary>
/// Generic endpoint для получения записи по ID
/// </summary>
public class GetByIdEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Results<Ok<TModel>, NotFound>, TIdentity, ISender>
    where TModel : class
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            GetRoutePath(CrudAction.GetById, true),
            async (TIdentity id, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Get{ResourceName}ById",
                "Получить запись по идентификатору",
                "Возвращает запись по уникальному идентификатору"
            )
            .ProducesGet<TModel>(hasNotFound: true);
    }

    public virtual async Task<Results<Ok<TModel>, NotFound>> HandleAsync(
        TIdentity id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetByIdQuery<TModel, TIdentity>(id);
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
