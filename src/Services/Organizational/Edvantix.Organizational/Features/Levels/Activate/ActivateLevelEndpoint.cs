namespace Edvantix.Organizational.Features.Levels.Activate;

public sealed class ActivateLevelEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/levels/{id:guid}/activate",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ActivateLevel")
            .WithTags("Уровни")
            .WithSummary("Активировать уровень")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new ActivateLevelCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
