using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка кабинетов организации.
/// Поддерживает фильтрацию по архивности и поиск по названию.
/// </summary>
public sealed class RoomListSpecification : Specification<Room>
{
    public RoomListSpecification(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId);

        if (!includeArchived)
            Query.Where(r => !r.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(r => r.Name.ToLower().Contains(term));
        }

        Query.OrderBy(r => r.Order).ThenBy(r => r.Name);

        SpecificationExtensions<Room>.ApplyPaging(Query, page, pageSize);
    }
}
