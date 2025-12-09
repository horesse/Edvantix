using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Queries;

/// <summary>
/// Generic endpoint для проверки существования записи
/// </summary>
public class IsExistEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Ok<bool>, TIdentity, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            $"/{ResourceName}/{{id}}/exists",
            async (TIdentity id, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Check{typeof(TModel).Name}Exists",
            $"Check if {typeof(TModel).Name} exists",
            $"Checks whether a {typeof(TModel).Name} with the specified ID exists"
        ).ProducesGet<bool>();
    }

    public virtual async Task<Ok<bool>> HandleAsync(
        TIdentity id,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var query = new IsExistQuery<TIdentity>(id);
        var result = await sender.Send(query, cancellationToken);
        
        return TypedResults.Ok(result);
    }
}
