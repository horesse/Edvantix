namespace Edvantix.Persona.Features.Profiles.UpdatePersonalInfo;

/// <summary>PATCH /v1/profile/personal-info — обновить личную информацию.</summary>
public sealed class UpdatePersonalInfoEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, UpdatePersonalInfoCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile/personal-info",
                async (
                    UpdatePersonalInfoCommand command,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, ct)
            )
            .WithName("UpdatePersonalInfo")
            .WithTags("Profile")
            .WithSummary("Обновить личную информацию")
            .WithDescription("Обновляет ФИО и дату рождения текущего пользователя.")
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        UpdatePersonalInfoCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
