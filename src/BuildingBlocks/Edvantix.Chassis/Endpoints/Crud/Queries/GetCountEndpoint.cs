using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Constants.Other;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Queries;

/// <summary>
/// Generic endpoint для получения количества записей
/// </summary>
public class GetCountEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Ok<long>, ISender>
    where TModel : class
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            GetRoutePath(CrudAction.GetCount),
            async (ISender sender, CancellationToken ct) => await HandleAsync(sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Get{ResourceName}Count",
                "Получить количество записей",
                "Возвращает общее количество записей"
            )
            .ProducesGet<long>();
    }

    public virtual async Task<Ok<long>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetCountQuery();
        var result = await sender.Send(query, cancellationToken);

        return TypedResults.Ok(result);
    }
}
