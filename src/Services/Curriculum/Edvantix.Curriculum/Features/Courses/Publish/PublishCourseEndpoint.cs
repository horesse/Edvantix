namespace Edvantix.Curriculum.Features.Courses.Publish;

/// <summary>POST /api/v1/courses/{id}/publish — опубликовать курс.</summary>
public sealed class PublishCourseEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/courses/{id:guid}/publish",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithTags("Курсы")
            .WithSummary("Опубликовать курс")
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
        await sender.Send(new PublishCourseCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
