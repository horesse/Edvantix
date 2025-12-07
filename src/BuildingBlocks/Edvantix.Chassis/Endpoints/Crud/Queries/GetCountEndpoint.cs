using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
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
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            $"/{ResourceName}/count",
            async (ISender sender, CancellationToken ct) =>
                await HandleAsync(sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Get{typeof(TModel).Name}Count",
            $"Get {typeof(TModel).Name} count",
            $"Returns the total count of {typeof(TModel).Name} records"
        ).ProducesGet<long>();
    }

    public virtual async Task<Ok<long>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCountQuery();
        var result = await sender.Send(query, cancellationToken);
        
        return TypedResults.Ok(result);
    }
}
