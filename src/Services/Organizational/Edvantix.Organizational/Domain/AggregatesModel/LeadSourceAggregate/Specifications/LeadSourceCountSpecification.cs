namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта источников привлечения организации.
/// <para>
/// <paramref name="isArchived"/> = <see langword="false"/> — только активные,
/// <paramref name="isArchived"/> = <see langword="true"/> — только архивные,
/// <paramref name="isArchived"/> = <see langword="null"/> — все записи.
/// </para>
/// </summary>
public sealed class LeadSourceCountSpecification : Specification<LeadSource>
{
    public LeadSourceCountSpecification(
        Guid organizationId,
        bool? isArchived = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(ls => ls.OrganizationId == organizationId);

        if (isArchived.HasValue)
            Query.Where(ls => ls.IsArchived == isArchived.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(ls => ls.Name.ToLower().Contains(term));
        }
    }
}
