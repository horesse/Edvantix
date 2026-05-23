namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности названия источника привлечения
/// среди активных записей организации.
/// </summary>
public sealed class LeadSourceUniqueNameSpecification : Specification<LeadSource>
{
    public LeadSourceUniqueNameSpecification(
        Guid organizationId,
        string name,
        Guid? excludeId = null
    )
    {
        Query
            .AsNoTracking()
            .Where(ls => ls.OrganizationId == organizationId && !ls.IsArchived && ls.Name == name);

        if (excludeId.HasValue)
            Query.Where(ls => ls.Id != excludeId.Value);
    }
}
