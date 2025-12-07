using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Commands;

/// <summary>
/// Generic endpoint для создания нескольких записей
/// </summary>
public class CreateRangeEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Created<IEnumerable<TIdentity>>, IEnumerable<TModel>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            $"/{ResourceName}/batch",
            async (IEnumerable<TModel> models, ISender sender, CancellationToken ct) =>
                await HandleAsync(models, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Create{typeof(TModel).Name}Batch",
            $"Create multiple {typeof(TModel).Name}s",
            $"Creates multiple {typeof(TModel).Name} records in a single operation"
        ).ProducesPost<IEnumerable<TIdentity>>();
    }

    public virtual async Task<Created<IEnumerable<TIdentity>>> HandleAsync(
        IEnumerable<TModel> models,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateRangeCommand<TModel, TIdentity>(models);
        var ids = await sender.Send(command, cancellationToken);
        
        return TypedResults.Created($"/{ResourceName}/batch", ids);
    }
}

