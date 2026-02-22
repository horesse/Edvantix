using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>PUT /v1/profile — обновить собственный профиль, возвращает обновлённый ProfileViewModel.</summary>
public sealed class UpdateOwnProfileEndpoint
    : IEndpoint<Ok<ProfileViewModel>, UpdateOwnProfileCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile",
                async (
                    [FromForm] UpdateOwnProfileCommand command,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, ct)
            )
            .Accepts<UpdateOwnProfileCommand>(MediaTypeNames.Multipart.FormData)
            .WithName("UpdateOwnProfile")
            .WithTags("Profile")
            .WithSummary("Обновить собственный профиль")
            .WithDescription(
                "Обновляет персональные данные, контакты, образование и опыт работы. "
                    + "Принимает multipart/form-data. Коллекции заменяются целиком. "
                    + "Аватар (JPEG/PNG, до 1 МБ) передаётся в поле avatar."
            )
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithFormOptions(true)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        UpdateOwnProfileCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
