namespace Edvantix.Curriculum.Features.Lessons.Publish;

/// <summary>POST /api/v1/lessons/{id}/publish — опубликовать урок.</summary>
public sealed class PublishLessonEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/lessons/{id:guid}/publish",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithTags("Уроки")
            .WithSummary("Опубликовать урок")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(new PublishLessonCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
