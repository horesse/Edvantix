namespace Edvantix.Curriculum.Features.Lessons.Update;

/// <summary>PUT /api/v1/lessons/{id} — обновить поля урока.</summary>
public sealed class UpdateLessonEndpoint : IEndpoint<NoContent, UpdateLessonCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/lessons/{id:guid}",
                async (
                    Guid id,
                    UpdateLessonCommand command,
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
            .WithSummary("Обновить урок")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateLessonCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
