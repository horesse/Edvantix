namespace Edvantix.Persona.Features.Profiles.UpdateProfile;

/// <summary>PATCH /v1/profile — единый метод обновления профиля.</summary>
public sealed class UpdateProfileEndpoint
    : IEndpoint<Ok<Guid>, UpdateProfileCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile",
                async (
                    UpdateProfileCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, cancellationToken)
            )
            .WithName("UpdateProfile")
            .WithTags("Profile")
            .WithSummary("Обновить профиль")
            .WithDescription(
                "Заменяет все редактируемые данные профиля: личные данные, контакты, опыт работы, образование и навыки."
            )
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<Guid>> HandleAsync(
        UpdateProfileCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
