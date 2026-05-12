using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;
using Grpc.Core;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Curriculum.Grpc.Services.Curriculum;

/// <summary>
/// gRPC-сервис каталога программ.
/// Используется Organizational-сервисом для валидации и отображения курсов при создании групп.
/// </summary>
internal sealed class CurriculumCatalogService(ICourseRepository repository)
    : CurriculumGrpcService.CurriculumGrpcServiceBase
{
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetCoursesForOrganizationResponse> GetCoursesForOrganization(
        GetCoursesForOrganizationRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.OrganizationId, out var organizationId))
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Некорректный идентификатор организации.")
            );

        var spec = new CourseOptionsSpecification(organizationId, request.Search);
        var courses = await repository.ListAsync(spec, context.CancellationToken);

        var response = new GetCoursesForOrganizationResponse();
        response.Courses.AddRange(
            courses.Select(c => new CourseOption
            {
                Id = c.Id.ToString(),
                Code = c.Code,
                Name = c.Name,
                Level = c.Level,
                Subject = c.Subject.ToString(),
            })
        );

        return response;
    }

    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetCourseByIdResponse> GetCourseById(
        GetCourseByIdRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.CourseId, out var courseId))
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Некорректный идентификатор курса.")
            );

        var course = await repository.GetByIdAsync(courseId, context.CancellationToken);

        if (course is null)
            return new GetCourseByIdResponse { Found = false };

        return new GetCourseByIdResponse
        {
            Found = true,
            Course = new CourseInfo
            {
                Id = course.Id.ToString(),
                OrganizationId = course.OrganizationId.ToString(),
                Code = course.Code,
                Name = course.Name,
                Subject = course.Subject.ToString(),
                Level = course.Level,
                DurationWeeks = course.DurationWeeks,
                Status = course.Status.ToString(),
            },
        };
    }
}
