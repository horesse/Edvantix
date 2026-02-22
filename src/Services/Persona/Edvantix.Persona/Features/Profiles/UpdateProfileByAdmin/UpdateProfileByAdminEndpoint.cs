using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.UpdateProfileByAdmin;

/// <summary>PUT /v1/profiles/{id} — обновить профиль пользователя (только администратор).</summary>
public sealed class UpdateProfileByAdminEndpoint
    : IEndpoint<Ok<ProfileViewModel>, UpdateProfileByAdminCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profiles/{id:long}",
                async (
                    Guid id,
                    [FromForm] UpdateProfileByAdminCommand command,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    command.ProfileId = id;
                    return await HandleAsync(command, sender, ct);
                }
            )
            .Accepts<UpdateProfileByAdminCommand>(MediaTypeNames.Multipart.FormData)
            .WithName("UpdateProfileByAdmin")
            .WithTags("Profile")
            .WithSummary("Обновить профиль (администратор)")
            .WithDescription(
                "Позволяет администратору обновить профиль любого пользователя. "
                    + "Принимает multipart/form-data. Аватар (JPEG/PNG, до 1 МБ) передаётся в поле avatar."
            )
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithFormOptions(true)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        UpdateProfileByAdminCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
