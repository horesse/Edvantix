namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>Спецификация для загрузки активных кабинетов организации с отслеживанием изменений.</summary>
public sealed class RoomReorderSpec : Specification<Room>
{
    public RoomReorderSpec(Guid organizationId)
    {
        Query.Where(r => r.OrganizationId == organizationId).OrderBy(r => r.Order).AsTracking();
    }
}
