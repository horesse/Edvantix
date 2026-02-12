using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.UpdateAvatar;

/// <summary>
/// Эндпоинт для обновления аватара текущего пользователя
/// </summary>
public class UpdateAvatarEndpoint : IEndpoint<NoContent, UpdateAvatarCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/profile/avatar",
                async (IFormFile image, ISender sender, CancellationToken ct) =>
                {
                    var command = new UpdateAvatarCommand { Image = image };
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdateAvatar")
            .WithTags("Profile")
            .WithSummary("Обновить аватар текущего пользователя")
            .WithDescription("Позволяет пользователю загрузить новый аватар")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization()
            .DisableAntiforgery();
    }

    public async Task<NoContent> HandleAsync(
        UpdateAvatarCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
