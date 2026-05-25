namespace Edvantix.Organizational.Features.Directories.LessonTypes.Archive;

/// <summary>POST /api/v1/directories/lesson-types/{id}/archive — архивировать тип занятия.</summary>
public sealed class ArchiveLessonTypeEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/lesson-types/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("ArchiveLessonType")
            .WithTags("Справочник: Типы занятий")
            .WithSummary("Архивировать тип занятия")
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
        await sender.Send(new ArchiveLessonTypeCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
