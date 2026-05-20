using Edvantix.Curriculum.Grpc.Services;

namespace Edvantix.Groups.Grpc.Services.Courses;

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

    public async Task<IReadOnlyDictionary<Guid, CourseRefDto>> GetCoursesByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    )
    {
        var request = new GetCoursesByIdsRequest();
        request.CourseIds.AddRange(ids.Select(id => id.ToString()));

        var response = await client.GetCoursesByIdsAsync(
            request,
            cancellationToken: cancellationToken
        );

        return response.Courses.ToDictionary(
            c => Guid.Parse(c.Id),
            c => new CourseRefDto(Guid.Parse(c.Id), c.Code, c.Name)
        );
    }
}
