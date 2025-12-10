using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Constants.Other;
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
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapDelete(
            GetRoutePath(CrudAction.DeleteRange),
            async ([FromBody] IEnumerable<TIdentity> ids, ISender sender, CancellationToken ct) =>
            await HandleAsync(ids, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Delete{ResourceName}Batch",
            $"Удалить несколько записей",
            $"Удаляет несколько записей по их идентификаторам"
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
