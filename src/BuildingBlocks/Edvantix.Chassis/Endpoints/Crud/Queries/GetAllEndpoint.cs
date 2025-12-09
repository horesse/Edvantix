using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Queries;

/// <summary>
/// Generic endpoint для получения всех записей
/// </summary>
public class GetAllEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Ok<IEnumerable<TModel>>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet(
            $"/{ResourceName}",
            async (ISender sender, CancellationToken ct) =>
                await HandleAsync(sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"GetAll{typeof(TModel).Name}s",
            $"Get all {typeof(TModel).Name}s",
            $"Retrieves all {typeof(TModel).Name} records"
        ).ProducesGet<IEnumerable<TModel>>();
    }

    public virtual async Task<Ok<IEnumerable<TModel>>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllQuery<TModel, TIdentity>();
        var result = await sender.Send(query, cancellationToken);
        
        return TypedResults.Ok(result);
    }
}
