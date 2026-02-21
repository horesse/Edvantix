namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>PUT /v1/profile — обновить собственный профиль, возвращает обновлённый ProfileViewModel.</summary>
public sealed class UpdateOwnProfileEndpoint
    : IEndpoint<Ok<ProfileViewModel>, UpdateOwnProfileCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/profile",
                async (UpdateProfileRequest request, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new UpdateOwnProfileCommand(request), sender, ct)
            )
            .WithName("UpdateOwnProfile")
            .WithTags("Profile")
            .WithSummary("Обновить собственный профиль")
            .WithDescription(
                "Обновляет персональные данные, контакты, образование и опыт работы. "
                    + "Коллекции заменяются целиком. Аватар обновляется через PUT /profile/avatar."
            )
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
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
