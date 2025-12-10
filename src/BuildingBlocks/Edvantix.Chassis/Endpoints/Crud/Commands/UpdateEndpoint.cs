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
/// Generic endpoint для обновления записи
/// </summary>
public class UpdateEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Results<NoContent, NotFound>, TIdentity, TModel, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPut(
            GetRoutePath(CrudAction.Update, true),
            async (TIdentity id, TModel model, ISender sender, CancellationToken ct) =>
                await HandleAsync(id, model, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Update{ResourceName}",
            $"Обновить запись",
            $"Обновляет существующую запись"
        ).ProducesPut();
    }

    public virtual async Task<Results<NoContent, NotFound>> HandleAsync(
        TIdentity id,
        TModel model,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        // Убедимся что ID совпадает
        model.Id = id;
        
        var command = new UpdateCommand<TModel, TIdentity>(model);
        await sender.Send(command, cancellationToken);
        
        return TypedResults.NoContent();
    }
}
