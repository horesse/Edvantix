using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses.Options;

/// <summary>
/// Лёгкий список курсов для выпадающих списков (например, «Курс / программа» в Group Create).
/// Возвращает только Active-курсы организации.
/// </summary>
public sealed record GetCourseOptionsQuery(string? Search = null)
    : IQuery<IReadOnlyList<CourseOptionDto>>;

internal sealed class GetCourseOptionsQueryHandler(
    ITenantContext tenantContext,
    CurriculumDbContext context
) : IQueryHandler<GetCourseOptionsQuery, IReadOnlyList<CourseOptionDto>>
{
    public async ValueTask<IReadOnlyList<CourseOptionDto>> Handle(
        GetCourseOptionsQuery query,
        CancellationToken cancellationToken
    )
    {
        var q = context
            .Courses.AsNoTracking()
            .Where(c =>
                c.OrganizationId == tenantContext.OrganizationId && c.Status == CourseStatus.Active
            );

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(c =>
                c.Name.Contains(query.Search) || c.Code.Contains(query.Search.ToUpperInvariant())
            );

        var options = await q.OrderBy(c => c.Code)
            .Select(c => new CourseOptionDto(c.Id, c.Code, c.Name, c.Level, c.Subject))
            .ToListAsync(cancellationToken);

        return options;
    }
}
