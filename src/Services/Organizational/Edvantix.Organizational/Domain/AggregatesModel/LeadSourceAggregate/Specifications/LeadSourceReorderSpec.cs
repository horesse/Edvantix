namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>Спецификация для загрузки активных источников привлечения организации с отслеживанием изменений.</summary>
public sealed class LeadSourceReorderSpec : Specification<LeadSource>
{
    public LeadSourceReorderSpec(Guid organizationId)
    {
        Query.Where(ls => ls.OrganizationId == organizationId).OrderBy(ls => ls.Order).AsTracking();
    }
}
