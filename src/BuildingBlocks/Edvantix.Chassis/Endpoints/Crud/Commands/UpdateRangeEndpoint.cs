using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Commands;

/// <summary>
/// Generic endpoint для обновления нескольких записей
/// </summary>
public class UpdateRangeEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<NoContent, IEnumerable<TModel>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPut(
            $"/{ResourceName}/batch",
            async (IEnumerable<TModel> models, ISender sender, CancellationToken ct) =>
                await HandleAsync(models, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Update{typeof(TModel).Name}Batch",
            $"Update multiple {typeof(TModel).Name}s",
            $"Updates multiple {typeof(TModel).Name} records in a single operation"
        ).ProducesPut();
    }

    public virtual async Task<NoContent> HandleAsync(
        IEnumerable<TModel> models,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateRangeCommand<TModel, TIdentity>(models);
        await sender.Send(command, cancellationToken);
        
        return TypedResults.NoContent();
    }
}
