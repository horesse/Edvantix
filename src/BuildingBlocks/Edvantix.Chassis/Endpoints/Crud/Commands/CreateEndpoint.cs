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
/// Generic endpoint для создания записи
/// </summary>
public class CreateEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Created<TIdentity>, TModel, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            GetRoutePath(CrudAction.Create),
            async (TModel model, ISender sender, CancellationToken ct) =>
                await HandleAsync(model, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Create{ResourceName}",
                $"Создать запись",
                $"Создать новую запись"
            )
            .ProducesPost<TIdentity>();
    }

    public virtual async Task<Created<TIdentity>> HandleAsync(
        TModel model,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateCommand<TModel, TIdentity>(model);
        var id = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"{GetRoutePath(CrudAction.GetById)}/{id}", id);
    }
}
