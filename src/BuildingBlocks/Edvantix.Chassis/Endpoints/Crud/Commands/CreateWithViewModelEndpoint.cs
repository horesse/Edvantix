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
/// Generic endpoint для создания записи с использованием CreateViewModel
/// </summary>
public class CreateWithViewModelEndpoint<TModel, TCreateViewModel, TIdentity>
    : BaseCrudViewModelEndpoint<TModel, TIdentity>,
        IEndpoint<Created<TIdentity>, TCreateViewModel, ISender>
    where TModel : Model<TIdentity>
    where TCreateViewModel : class
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            GetRoutePath(CrudAction.Create),
            async (TCreateViewModel viewModel, ISender sender, CancellationToken ct) =>
                await HandleAsync(viewModel, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Create{ResourceName}",
                $"Создать запись",
                $"Создать новую запись используя CreateViewModel"
            )
            .ProducesPost<TIdentity>();
    }

    public virtual async Task<Created<TIdentity>> HandleAsync(
        TCreateViewModel viewModel,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new CreateWithViewModelCommand<TModel, TCreateViewModel, TIdentity>(
            viewModel
        );
        var id = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"{GetRoutePath(CrudAction.GetById)}/{id}", id);
    }
}
