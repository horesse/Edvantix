using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Constants.Other;
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
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapDelete(
            GetRoutePath(CrudAction.Delete, true),
            async (TIdentity id, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Delete{ResourceName}",
            $"Удалить запись",
            $"Удаляет запись по идентификатору"
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
