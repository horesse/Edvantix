namespace Edvantix.Curriculum.Features.Courses.Options;

/// <summary>GET /api/v1/courses/options — список активных курсов для дропдаунов.</summary>
public sealed class GetCourseOptionsEndpoint
    : IEndpoint<Ok<IReadOnlyList<CourseOptionDto>>, GetCourseOptionsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/courses/options",
                async (string? search, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(new GetCourseOptionsQuery(search), sender, cancellationToken)
            )
            .WithName("GetCourseOptions")
            .WithTags("Курсы")
            .WithSummary("Список активных курсов для выпадающего списка")
            .Produces<IReadOnlyList<CourseOptionDto>>()
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<CourseOptionDto>>> HandleAsync(
        GetCourseOptionsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
