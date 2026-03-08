using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>PATCH /v1/profile/avatar — загрузить или заменить аватар профиля.</summary>
public sealed class UpdateAvatarEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, UpdateAvatarCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile/avatar",
                async (
                    [FromForm] UpdateAvatarCommand command,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, ct)
            )
            .Accepts<UpdateAvatarCommand>(MediaTypeNames.Multipart.FormData)
            .WithName("UpdateAvatar")
            .WithTags("Profile")
            .WithSummary("Загрузить или заменить аватар")
            .WithDescription(
                "Загружает новый аватар профиля (JPEG/PNG, до 1 МБ). "
                    + "Предыдущий аватар автоматически удаляется."
            )
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithFormOptions(true)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        UpdateAvatarCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
