namespace Edvantix.Curriculum.Features.Lessons.Move;

/// <summary>POST /api/v1/lessons/{id}/move — переместить урок на новую позицию в модуле.</summary>
public sealed class MoveLessonEndpoint : IEndpoint<NoContent, MoveLessonCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/lessons/{id:guid}/move",
                async (
                    Guid id,
                    MoveLessonCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        command with { LessonId = id },
                        sender,
                        cancellationToken
                    )
            )
            .WithTags("Уроки")
            .WithSummary("Переместить урок на новую позицию")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        MoveLessonCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
