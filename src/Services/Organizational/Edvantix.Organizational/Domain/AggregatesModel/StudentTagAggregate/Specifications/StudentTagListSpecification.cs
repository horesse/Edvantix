using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка тегов студентов организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class StudentTagListSpecification : Specification<StudentTag>
{
    public StudentTagListSpecification(
        Guid organizationId,
        bool isArchive,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(t => t.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(t => t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(t => t.Name.ToLower().Contains(search.Trim().ToLower()));

        Query.OrderBy(t => t.Order).ThenBy(t => t.Name);

        SpecificationExtensions<StudentTag>.ApplyPaging(Query, page, pageSize);
    }
}
