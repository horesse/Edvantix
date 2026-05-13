using Edvantix.Curriculum.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Courses;

[ExcludeFromCodeCoverage]
internal sealed class CurriculumService(CurriculumGrpcService.CurriculumGrpcServiceClient client)
    : ICurriculumService
{
    public async Task<CourseInfo?> GetCourseByIdAsync(
        string courseId,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.GetCourseByIdAsync(
            new GetCourseByIdRequest { CourseId = courseId },
            cancellationToken: cancellationToken
        );

        return response.Found ? response.Course : null;
    }

    public async Task<IReadOnlyList<CourseOption>> GetCoursesForOrganizationAsync(
        string organizationId,
        string? search = null,
        CancellationToken cancellationToken = default
    )
    {
        var response = await client.GetCoursesForOrganizationAsync(
            new GetCoursesForOrganizationRequest
            {
                OrganizationId = organizationId,
                Search = search ?? string.Empty,
            },
            cancellationToken: cancellationToken
        );

        return [.. response.Courses];
    }
}
