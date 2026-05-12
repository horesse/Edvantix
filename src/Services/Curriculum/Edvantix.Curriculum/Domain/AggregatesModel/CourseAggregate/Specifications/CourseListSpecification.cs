using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;

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
        Query.AsNoTracking().OrderBy(c => c.Code).Skip(offset).Take(limit);
        ApplyFilters(Query, organizationId, search, subject, status);
    }

    internal static void ApplyFilters(
        ISpecificationBuilder<Course> query,
        Guid organizationId,
        string? search,
        CourseSubject? subject,
        CourseStatus? status
    )
    {
        query.Where(c => c.OrganizationId == organizationId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Search(c => c.Name, $"%{search}%", 1);
            query.Search(c => c.Code, $"%{search.ToUpperInvariant()}%", 1);
        }

        if (subject.HasValue)
            query.Where(c => c.Subject == subject.Value);

        if (status.HasValue)
            query.Where(c => c.Status == status.Value);
    }
}

/// <summary>Счётчик курсов для пагинации (без Skip/Take).</summary>
internal sealed class CourseCountSpecification : Specification<Course>
{
    public CourseCountSpecification(
        Guid organizationId,
        string? search,
        CourseSubject? subject,
        CourseStatus? status
    )
    {
        CourseListSpecification.ApplyFilters(Query, organizationId, search, subject, status);
    }
}
