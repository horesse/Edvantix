namespace Edvantix.Persona.Features.Profiles.UpdateEducation;

/// <summary>PATCH /v1/profile/education — обновить образование.</summary>
public sealed class UpdateEducationEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, UpdateEducationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile/education",
                async (
                    UpdateEducationCommand command,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, ct)
            )
            .WithName("UpdateEducation")
            .WithTags("Profile")
            .WithSummary("Обновить образование")
            .WithDescription("Заменяет все записи об образовании текущего пользователя.")
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        UpdateEducationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
