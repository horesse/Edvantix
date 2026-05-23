using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка тегов студентов организации.
/// Поддерживает фильтрацию по архивности и поиск по названию.
/// </summary>
public sealed class StudentTagListSpecification : Specification<StudentTag>
{
    public StudentTagListSpecification(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(t => t.OrganizationId == organizationId);

        if (!includeArchived)
            Query.Where(t => !t.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(t => t.Name.ToLower().Contains(term));
        }

        Query.OrderBy(t => t.Order).ThenBy(t => t.Name);

        SpecificationExtensions<StudentTag>.ApplyPaging(Query, page, pageSize);
    }
}
