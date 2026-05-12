namespace Edvantix.Curriculum.Features.Courses.Get;

/// <summary>GET /api/v1/courses/{id} — детальная страница курса.</summary>
public sealed class GetCourseByIdEndpoint : IEndpoint<Ok<CourseDetailDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/courses/{id:guid}",
                async (Guid id, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(id, sender, cancellationToken)
            )
            .WithName("GetCourseById")
            .WithTags("Курсы")
            .WithSummary("Детальная страница курса")
            .Produces<CourseDetailDto>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<CourseDetailDto>> HandleAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetCourseByIdQuery(id), cancellationToken);
        return TypedResults.Ok(result);
    }
}
