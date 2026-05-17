namespace Edvantix.Groups.Features.Levels.Deactivate;

public sealed class DeactivateLevelEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/levels/{id:guid}/deactivate",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("DeactivateLevel")
            .WithTags("Уровни")
            .WithSummary("Деактивировать уровень")
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
        await sender.Send(new DeactivateLevelCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
