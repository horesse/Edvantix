namespace Edvantix.Persona.Features.Admin.Profiles.Unblock;

public sealed class UnblockProfileEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/profiles/{profileId:guid}/unblock",
                async (Guid profileId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(profileId, sender, cancellationToken)
            )
            .WithName("Разблокировать профиль")
            .WithTags("Администрирование")
            .WithSummary("Снятие блокировки профиля пользователя")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        Guid profileId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new UnblockProfileCommand(profileId), cancellationToken);
        return TypedResults.NoContent();
    }
}
