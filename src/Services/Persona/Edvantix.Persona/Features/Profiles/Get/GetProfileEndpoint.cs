namespace Edvantix.Persona.Features.Profiles.Get;

public sealed class GetProfileEndpoint : IEndpoint<Ok<ProfileDto>, ISender>
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
            .Produces<ProfileDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDto>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetProfileQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
