using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
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
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            $"/{ResourceName}/{{id}}",
            async (TIdentity id, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Get{typeof(TModel).Name}ById",
            $"Get {typeof(TModel).Name} by ID",
            $"Retrieves a single {typeof(TModel).Name} record by its unique identifier"
        ).ProducesGet<TModel>(hasNotFound: true);
    }

    public virtual async Task<Results<Ok<TModel>, NotFound>> HandleAsync(
        TIdentity id,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var query = new GetByIdQuery<TModel, TIdentity>(id);
        var result = await sender.Send(query, cancellationToken);
        
        return TypedResults.Ok(result);
    }
}
