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
/// Generic endpoint для создания нескольких записей
/// </summary>
public class CreateRangeEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Created<IEnumerable<TIdentity>>, IEnumerable<TModel>, ISender>
    where TModel : Model<TIdentity>
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            GetRoutePath(CrudAction.CreateRange),
            async (IEnumerable<TModel> models, ISender sender, CancellationToken ct) =>
                await HandleAsync(models, sender, ct)
        );

        ConfigureEndpoint(
            builder,
            $"Create{ResourceName}Batch",
            $"Создать несколько записей",
            $"Создаёт несколько записей за одну операцию"
        ).ProducesPost<IEnumerable<TIdentity>>();
    }

    public virtual async Task<Created<IEnumerable<TIdentity>>> HandleAsync(
        IEnumerable<TModel> models,
        ISender sender,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateRangeCommand<TModel, TIdentity>(models);
        var ids = await sender.Send(command, cancellationToken);
        
        return TypedResults.Created($"{GetRoutePath(CrudAction.GetById)}", ids);
    }
}

