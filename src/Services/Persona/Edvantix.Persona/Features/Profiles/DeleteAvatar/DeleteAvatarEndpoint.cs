namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

/// <summary>DELETE /v1/profile/avatar — удалить аватар профиля.</summary>
public sealed class DeleteAvatarEndpoint : IEndpoint<Ok<Guid>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/profile/avatar",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("DeleteAvatar")
            .WithTags("Profile")
            .WithSummary("Удалить аватар")
            .WithDescription(
                "Удаляет текущий аватар профиля из хранилища и очищает поле аватара в профиле."
            )
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<Guid>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new DeleteAvatarCommand(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
