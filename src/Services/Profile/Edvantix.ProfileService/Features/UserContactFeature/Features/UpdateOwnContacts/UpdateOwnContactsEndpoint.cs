using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.UserContactFeature.Features.UpdateOwnContacts;

/// <summary>
/// Эндпоинт для обновления контактов текущего пользователя
/// </summary>
public class UpdateOwnContactsEndpoint : IEndpoint<NoContent, UpdateOwnContactsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile/contacts",
                async (
                    IEnumerable<UserContactModel> contacts,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdateOwnContactsCommand(contacts);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOwnContacts")
            .WithTags("Profile")
            .WithSummary("Обновить контакты текущего пользователя")
            .WithDescription("Позволяет пользователю обновить свои контактные данные")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOwnContactsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
