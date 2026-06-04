namespace Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate.Specifications;

/// <summary>Спецификация для загрузки активных предметов организации с отслеживанием изменений.</summary>
public sealed class SubjectReorderSpec : Specification<Subject>
{
    public SubjectReorderSpec(Guid organizationId)
    {
        Query.Where(s => s.OrganizationId == organizationId).OrderBy(s => s.Order).AsTracking();
    }
}
