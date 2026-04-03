namespace Edvantix.Persona.Features.Admin.Profiles.TrackLogin;

public sealed class RecordLastLoginEndpoint : IEndpoint<NoContent, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/me/session",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("Отметить сессию")
            .WithTags("Профиль")
            .WithSummary("Обновить время последнего входа")
            .WithDescription("Вызывается фронтендом при старте сессии для отслеживания активности")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new RecordLastLoginCommand(), cancellationToken);
        return TypedResults.NoContent();
    }
}
