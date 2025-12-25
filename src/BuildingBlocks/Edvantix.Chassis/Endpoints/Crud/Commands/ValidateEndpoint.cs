using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Constants.Other;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Edvantix.Chassis.Endpoints.Crud.Commands;

/// <summary>
/// Generic endpoint для валидации модели
/// </summary>
public class ValidateEndpoint<TModel, TIdentity>
    : BaseCrudEndpoint<TModel, TIdentity>,
        IEndpoint<Ok<bool>, TModel, ISender>
    where TModel : class
    where TIdentity : struct
{
    public virtual void MapEndpoint(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost(
            GetRoutePath(CrudAction.Validate),
            async (TModel model, ISender sender, CancellationToken ct) =>
                await HandleAsync(model, sender, ct)
        );

        ConfigureEndpoint(
                builder,
                $"Validate{ResourceName}",
                "Валидировать модель",
                "Проверяет валидность модели без её сохранения. FluentValidation применяется автоматически через pipeline."
            )
            .Produces<bool>();
    }

    public virtual async Task<Ok<bool>> HandleAsync(
        TModel model,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ValidateCommand<TModel>(model);
        var isValid = await sender.Send(command, cancellationToken);

        return TypedResults.Ok(isValid);
    }
}
