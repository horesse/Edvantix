using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>PUT /v1/profile/avatar — загрузить/заменить аватар текущего пользователя.</summary>
public sealed class UpdateAvatarEndpoint
    : IEndpoint<Ok<ProfileViewModel>, UpdateAvatarCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile/avatar",
                async ([FromForm] IFormFile avatar, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new UpdateAvatarCommand(avatar), sender, ct)
            )
            .Accepts<IFormFile>(MediaTypeNames.Multipart.FormData)
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("UpdateAvatar")
            .WithTags("Profile")
            .WithSummary("Загрузить аватар")
            .WithDescription(
                "Загружает новый аватар в Azure Blob Storage. Старый аватар удаляется."
            )
            .WithFormOptions(true)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        UpdateAvatarCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
