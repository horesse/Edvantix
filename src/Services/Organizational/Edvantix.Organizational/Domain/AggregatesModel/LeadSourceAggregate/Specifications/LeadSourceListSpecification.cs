using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка источников привлечения организации.
/// Поддерживает фильтрацию по архивности и поиск по названию.
/// </summary>
public sealed class LeadSourceListSpecification : Specification<LeadSource>
{
    public LeadSourceListSpecification(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(ls => ls.OrganizationId == organizationId);

        if (!includeArchived)
            Query.Where(ls => !ls.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(ls => ls.Name.ToLower().Contains(term));
        }

        Query.OrderBy(ls => ls.Order).ThenBy(ls => ls.Name);

        SpecificationExtensions<LeadSource>.ApplyPaging(Query, page, pageSize);
    }
}
