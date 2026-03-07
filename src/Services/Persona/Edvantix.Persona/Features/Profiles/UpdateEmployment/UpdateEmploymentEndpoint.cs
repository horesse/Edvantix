namespace Edvantix.Persona.Features.Profiles.UpdateEmployment;

/// <summary>PATCH /v1/profile/employment — обновить опыт работы.</summary>
public sealed class UpdateEmploymentEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, UpdateEmploymentCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile/employment",
                async (
                    UpdateEmploymentCommand command,
                    ISender sender,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, ct)
            )
            .WithName("UpdateEmployment")
            .WithTags("Profile")
            .WithSummary("Обновить опыт работы")
            .WithDescription("Заменяет все записи об опыте работы текущего пользователя.")
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        UpdateEmploymentCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
