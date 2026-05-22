namespace Edvantix.Groups.Features.Directories.Levels.Restore;

/// <summary>POST /api/v1/directories/levels/{id}/restore — восстановить уровень из архива.</summary>
public sealed class RestoreLevelDirectoryEndpoint
    : IEndpoint<Results<NoContent, NotFound>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/levels/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreLevelDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Восстановить уровень из архива (активировать)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Results<NoContent, NotFound>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new RestoreLevelDirectoryCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
