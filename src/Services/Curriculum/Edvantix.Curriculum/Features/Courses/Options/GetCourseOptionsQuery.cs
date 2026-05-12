using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;

namespace Edvantix.Curriculum.Features.Courses.Options;

/// <summary>
/// Лёгкий список курсов для выпадающих списков (например, «Курс / программа» в Group Create).
/// Возвращает только Active-курсы организации.
/// </summary>
public sealed record GetCourseOptionsQuery(string? Search = null)
    : IQuery<IReadOnlyList<CourseOptionDto>>;

internal sealed class GetCourseOptionsQueryHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : IQueryHandler<GetCourseOptionsQuery, IReadOnlyList<CourseOptionDto>>
{
    public async ValueTask<IReadOnlyList<CourseOptionDto>> Handle(
        GetCourseOptionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var spec = new CourseOptionsSpecification(tenantContext.OrganizationId, query.Search);
        var courses = await repository.ListAsync(spec, cancellationToken);

        return [.. courses.Select(c => new CourseOptionDto(c.Id, c.Code, c.Name, c.Level, c.Subject))];
    }
}
