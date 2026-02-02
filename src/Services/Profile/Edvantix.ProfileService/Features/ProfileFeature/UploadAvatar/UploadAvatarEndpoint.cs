using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.UploadAvatar;

/// <summary>
/// Эндпоинт для загрузки аватара пользователя
/// </summary>
public class UploadAvatarEndpoint : IEndpoint<NoContent, UploadAvatarCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/profile/avatar",
                async (IFormFile image, ISender sender, CancellationToken ct) =>
                {
                    var command = new UploadAvatarCommand(image);
                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UploadAvatar")
            .WithTags("Profile")
            .WithSummary("Загрузить аватар")
            .WithDescription(
                "Позволяет пользователю загрузить свой аватар. Принимает изображение в формате multipart/form-data"
            )
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status400BadRequest)
            .DisableAntiforgery()
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UploadAvatarCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
