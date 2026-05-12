namespace Edvantix.Curriculum.Features.Courses.Update;

/// <summary>PUT /api/v1/courses/{id} — обновить курс.</summary>
public sealed class UpdateCourseEndpoint : IEndpoint<NoContent, UpdateCourseCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/courses/{id:guid}",
                async (
                    Guid id,
                    UpdateCourseCommand command,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(command with { CourseId = id }, sender, cancellationToken)
            )
            .WithTags("Курсы")
            .WithSummary("Обновить курс")
            .Produces(StatusCodes.Status204NoContent)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<NoContent> HandleAsync(
        UpdateCourseCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
