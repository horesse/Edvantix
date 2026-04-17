using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Спецификация для постраничного получения организаций.
/// Поддерживает фильтрацию по поисковой строке, статусу и типу организации.
/// </summary>
public sealed class OrganizationListSpecification : Specification<Organization>
{
    public OrganizationListSpecification(
        int offset,
        int limit,
        string? search = null,
        OrganizationStatus? status = null,
        OrganizationType? organizationType = null
    )
    {
        Query
            .AsNoTracking()
            .Where(o => !o.IsDeleted)
            .OrderBy(o => o.FullLegalName)
            .Skip(offset)
            .Take(limit);

        ApplyFilters(Query, search, status, organizationType);
    }

    internal static void ApplyFilters(
        ISpecificationBuilder<Organization> query,
        string? search,
        OrganizationStatus? status,
        OrganizationType? organizationType
    )
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLowerInvariant();
            query.Where(o =>
                o.FullLegalName.ToLower().Contains(lower)
                || (o.ShortName != null && o.ShortName.ToLower().Contains(lower))
            );
        }

        if (status.HasValue)
        {
            query.Where(o => o.Status == status.Value);
        }

        if (organizationType.HasValue)
        {
            query.Where(o => o.OrganizationType == organizationType.Value);
        }
    }
}
