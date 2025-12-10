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
/// Generic endpoint для обновления нескольких записей
/// </summary>
public class UpdateRangeEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<NoContent, IEnumerable<TModel>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPut(
            GetRoutePath(CrudAction.UpdateRange),
            async (IEnumerable<TModel> models, ISender sender, CancellationToken ct) =>
                await HandleAsync(models, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Update{ResourceName}Batch",
                $"Обновить несколько записей",
                $"Обновляет несколько записей за одну операцию"
            )
            .ProducesPut();
    }

    public virtual async Task<NoContent> HandleAsync(
        IEnumerable<TModel> models,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new UpdateRangeCommand<TModel, TIdentity>(models);
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
