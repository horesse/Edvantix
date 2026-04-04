namespace Edvantix.Persona.Features.Admin.Profiles.Block;

public sealed class BlockProfileEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/profiles/{profileId:guid}/block",
                async (Guid profileId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(profileId, sender, cancellationToken)
            )
            .WithName("Заблокировать профиль")
            .WithTags("Администрирование")
            .WithSummary("Блокировка профиля пользователя")
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
        await sender.Send(new BlockProfileCommand(profileId), cancellationToken);
        return TypedResults.NoContent();
    }
}
