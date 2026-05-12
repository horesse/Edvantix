using Edvantix.Curriculum.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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
                    [FromQuery] int pageIndex = Pagination.DefaultPageIndex,
                    [FromQuery] int pageSize = Pagination.DefaultPageSize,
                    [FromQuery] string? search = null,
                    [FromQuery] CourseSubject? subject = null,
                    [FromQuery] CourseStatus? status = null,
                    ISender sender = default!,
                    CancellationToken cancellationToken = default
                ) =>
                    await HandleAsync(
                        new GetCoursesQuery(pageIndex, pageSize, search, subject, status),
                        sender,
                        cancellationToken
                    )
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
