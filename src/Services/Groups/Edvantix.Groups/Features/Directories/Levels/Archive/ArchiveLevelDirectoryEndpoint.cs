namespace Edvantix.Groups.Features.Directories.Levels.Archive;

/// <summary>POST /api/v1/directories/levels/{id}/archive — деактивировать уровень.</summary>
public sealed class ArchiveLevelDirectoryEndpoint
    : IEndpoint<Results<NoContent, NotFound>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/levels/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveLevelDirectory")
            .WithTags("Справочник: Уровни")
            .WithSummary("Перевести уровень в архив (деактивировать)")
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
        await sender.Send(new ArchiveLevelDirectoryCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
