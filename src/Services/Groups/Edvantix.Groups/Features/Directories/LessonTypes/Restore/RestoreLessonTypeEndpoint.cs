namespace Edvantix.Groups.Features.Directories.LessonTypes.Restore;

/// <summary>POST /api/v1/directories/lesson-types/{id}/restore — восстановить тип занятия из архива.</summary>
public sealed class RestoreLessonTypeEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/directories/lesson-types/{id:guid}/restore",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("RestoreLessonType")
            .WithTags("Типы занятий")
            .WithSummary("Восстановить тип занятия из архива")
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
        await sender.Send(new RestoreLessonTypeCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
