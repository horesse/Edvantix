using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка статусов студентов организации.
/// Поддерживает фильтрацию по архивности и поиск по имени/коду.
/// </summary>
public sealed class StudentStatusListSpecification : Specification<StudentStatus>
{
    public StudentStatusListSpecification(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(s => s.OrganizationId == organizationId);

        if (!includeArchived)
            Query.Where(s => !s.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(s => s.Name.ToLower().Contains(term) || s.Code.ToLower().Contains(term));
        }

        Query.OrderBy(s => s.Order).ThenBy(s => s.Name);

        SpecificationExtensions<StudentStatus>.ApplyPaging(Query, page, pageSize);
    }
}
