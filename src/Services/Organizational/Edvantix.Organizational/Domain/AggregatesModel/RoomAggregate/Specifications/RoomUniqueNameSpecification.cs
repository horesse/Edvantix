namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности названия кабинета среди активных записей организации.
/// </summary>
public sealed class RoomUniqueNameSpecification : Specification<Room>
{
    public RoomUniqueNameSpecification(Guid organizationId, string name, Guid? excludeId = null)
    {
        Query
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && !r.IsArchived && r.Name == name);

        if (excludeId.HasValue)
            Query.Where(r => r.Id != excludeId.Value);
    }
}
