namespace Edvantix.Persona.Features.Profiles.UpdateContacts;

/// <summary>PATCH /v1/profile/contacts — обновить контакты.</summary>
public sealed class UpdateContactsEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, UpdateContactsCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch(
                "/profile/contacts",
                async (UpdateContactsCommand command, ISender sender, CancellationToken ct) =>
                    await HandleAsync(command, sender, ct)
            )
            .WithName("UpdateContacts")
            .WithTags("Profile")
            .WithSummary("Обновить контакты")
            .WithDescription("Заменяет все контакты текущего пользователя переданным списком.")
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        UpdateContactsCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(command, cancellationToken);
        return TypedResults.Ok(result);
    }
}
