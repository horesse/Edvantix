namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;

/// <summary>Спецификация для загрузки уровней организации с отслеживанием изменений (для переупорядочивания).</summary>
public sealed class LevelReorderSpec : Specification<Level>
{
    public LevelReorderSpec(Guid organizationId)
    {
        Query.Where(l => l.OrganizationId == organizationId).OrderBy(l => l.SortOrder).AsTracking();
    }
}
