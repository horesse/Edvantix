namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта источников привлечения организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные,
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные.
/// </para>
/// </summary>
public sealed class LeadSourceCountSpecification : Specification<LeadSource>
{
    public LeadSourceCountSpecification(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(ls => ls.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(ls => ls.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(ls => ls.Name.ToLower().Contains(search.Trim().ToLower()));
    }
}
