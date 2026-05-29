using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка статусов студентов организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class StudentStatusListSpecification : Specification<StudentStatus>
{
    public StudentStatusListSpecification(
        Guid organizationId,
        bool isArchive,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(s => s.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(s => s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(s => s.Name.ToLower().Contains(term) || s.Code.ToLower().Contains(term));
        }

        Query.OrderBy(s => s.Order).ThenBy(s => s.Name);

        SpecificationExtensions<StudentStatus>.ApplyPaging(Query, page, pageSize);
    }
}
