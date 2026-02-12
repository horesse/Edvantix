using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.UpdateOwnProfile;

/// <summary>
/// Эндпоинт для обновления собственного профиля
/// </summary>
public class UpdateOwnProfileEndpoint : IEndpoint<NoContent, UpdateOwnProfileCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile",
                async (ProfileModel model, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateOwnProfileCommand(model);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateOwnProfile")
            .WithTags("Profile")
            .WithSummary("Обновить собственный профиль")
            .WithDescription("Позволяет пользователю обновить свой профиль")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateOwnProfileCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
