namespace Edvantix.Curriculum.Features.Courses.Archive;

/// <summary>POST /api/v1/courses/{id}/archive — архивировать курс.</summary>
public sealed class ArchiveCourseEndpoint : IEndpoint<NoContent, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/courses/{id:guid}/archive",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithTags("Курсы")
            .WithSummary("Архивировать курс")
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
        await sender.Send(new ArchiveCourseCommand(id), cancellationToken);
        return TypedResults.NoContent();
    }
}
