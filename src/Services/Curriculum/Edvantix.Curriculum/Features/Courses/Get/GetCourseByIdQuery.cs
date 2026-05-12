using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Courses.Get;

/// <summary>Детальная страница курса с модулями и уроками.</summary>
public sealed record GetCourseByIdQuery(Guid Id) : IQuery<CourseDetailDto>;

internal sealed class GetCourseByIdQueryHandler(
    ITenantContext tenantContext,
    ICourseRepository repository,
    IMapper<Course, CourseDetailDto> mapper
) : IQueryHandler<GetCourseByIdQuery, CourseDetailDto>
{
    public async ValueTask<CourseDetailDto> Handle(
        GetCourseByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdWithModulesAsync(query.Id, cancellationToken)
            ?? throw NotFoundException.For<Course>(query.Id);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(query.Id);

        return mapper.Map(course);
    }
}
