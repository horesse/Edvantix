using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Commands;

/// <summary>
/// Generic endpoint для удаления записи
/// </summary>
public class DeleteEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Results<NoContent, NotFound>, TIdentity, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    protected override string ResourceName => typeof(TModel).Name.ToLowerInvariant() + "s";
    protected override string Tag => typeof(TModel).Name;

    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapDelete(
            $"/{ResourceName}/{{id}}",
            async (TIdentity id, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Delete{typeof(TModel).Name}",
            $"Delete {typeof(TModel).Name}",
            $"Deletes a {typeof(TModel).Name} record by ID"
        ).ProducesDelete();
    }

    public virtual async Task<Results<NoContent, NotFound>> HandleAsync(
        TIdentity id,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCommand<TModel, TIdentity>(id);
        await sender.Send(command, cancellationToken);
        
        return TypedResults.NoContent();
    }
}
