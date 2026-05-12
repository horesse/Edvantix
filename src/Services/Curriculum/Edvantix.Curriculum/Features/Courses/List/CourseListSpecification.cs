using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses.List;

/// <summary>Постраничный список курсов организации с фильтрацией.</summary>
internal sealed class CourseListSpecification : Specification<Course>
{
    public CourseListSpecification(
        Guid organizationId,
        int offset,
        int limit,
        string? search,
        CourseSubject? subject,
        CourseStatus? status
    )
    {
        Query
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId)
            .OrderBy(c => c.Code)
            .Skip(offset)
            .Take(limit);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(c => c.Name.Contains(search) || c.Code.Contains(search.ToUpperInvariant()));

        if (subject.HasValue)
            Query.Where(c => c.Subject == subject.Value);

        if (status.HasValue)
            Query.Where(c => c.Status == status.Value);
    }
}

/// <summary>Счётчик курсов для пагинации.</summary>
internal sealed class CourseCountSpecification : Specification<Course>
{
    public CourseCountSpecification(
        Guid organizationId,
        string? search,
        CourseSubject? subject,
        CourseStatus? status
    )
    {
        Query.Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(c => c.Name.Contains(search) || c.Code.Contains(search.ToUpperInvariant()));

        if (subject.HasValue)
            Query.Where(c => c.Subject == subject.Value);

        if (status.HasValue)
            Query.Where(c => c.Status == status.Value);
    }
}
