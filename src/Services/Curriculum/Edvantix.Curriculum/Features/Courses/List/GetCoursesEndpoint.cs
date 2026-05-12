namespace Edvantix.Curriculum.Features.Courses.List;

/// <summary>GET /api/v1/courses — список курсов организации.</summary>
public sealed class GetCoursesEndpoint
    : IEndpoint<Ok<PagedResult<CourseDto>>, GetCoursesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/courses",
                async (
                    [AsParameters] GetCoursesQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(query, sender, cancellationToken)
            )
            .WithName("GetCourses")
            .WithTags("Курсы")
            .WithSummary("Список курсов организации")
            .Produces<PagedResult<CourseDto>>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<CourseDto>>> HandleAsync(
        GetCoursesQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
