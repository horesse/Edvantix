namespace Edvantix.Persona.Features.Profiles.GetProfile;

public sealed class GetProfileEndpoint : IEndpoint<Ok<ProfileViewModel>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profile",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("Получить профиль")
            .WithTags("Профиль")
            .WithSummary("Получить собственный профиль")
            .WithDescription("Возвращает краткую информацию о профиле текущего пользователя")
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetProfileQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
