using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses.List;

/// <summary>Постраничный список курсов организации.</summary>
public sealed record GetCoursesQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Размер страницы")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Поиск по названию или коду")] string? Search = null,
    [property: Description("Фильтр по предмету")] CourseSubject? Subject = null,
    [property: Description("Фильтр по статусу")] CourseStatus? Status = null
) : IQuery<PagedResult<CourseDto>>;

internal sealed class GetCoursesQueryHandler(
    ITenantContext tenantContext,
    ICourseRepository repository,
    IMapper<Course, CourseDto> mapper
) : IQueryHandler<GetCoursesQuery, PagedResult<CourseDto>>
{
    public async ValueTask<PagedResult<CourseDto>> Handle(
        GetCoursesQuery query,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(query.PageIndex, 1),
            PageSize: Math.Clamp(query.PageSize, 1, 100)
        );

        var offset = (clamped.PageIndex - 1) * clamped.PageSize;

        var listSpec = new CourseListSpecification(
            tenantContext.OrganizationId,
            offset,
            clamped.PageSize,
            query.Search,
            query.Subject,
            query.Status
        );

        var countSpec = new CourseCountSpecification(
            tenantContext.OrganizationId,
            query.Search,
            query.Subject,
            query.Status
        );

        var courses = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        return new PagedResult<CourseDto>(
            [.. courses.Select(mapper.Map)],
            clamped.PageIndex,
            clamped.PageSize,
            totalCount
        );
    }
}
