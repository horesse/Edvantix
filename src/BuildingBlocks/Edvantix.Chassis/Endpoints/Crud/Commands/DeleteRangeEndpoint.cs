using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Commands;

/// <summary>
/// Generic endpoint для удаления нескольких записей
/// </summary>
public class DeleteRangeEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<NoContent, IEnumerable<TIdentity>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapDelete(
            $"/{ResourceName}/batch",
            async ([FromBody] IEnumerable<TIdentity> ids, ISender sender, CancellationToken ct) =>
            await HandleAsync(ids, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Delete{typeof(TModel).Name}Batch",
            $"Delete multiple {typeof(TModel).Name}s",
            $"Deletes multiple {typeof(TModel).Name} records by their IDs"
        ).ProducesDelete();
    }

    public virtual async Task<NoContent> HandleAsync(
        IEnumerable<TIdentity> ids,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteRangeCommand<TModel, TIdentity>(ids);
        await sender.Send(command, cancellationToken);
        
        return TypedResults.NoContent();
    }
}
