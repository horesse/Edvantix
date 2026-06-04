using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка кабинетов организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class RoomListSpecification : Specification<Room>
{
    public RoomListSpecification(
        Guid organizationId,
        bool isArchive,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(r => r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(r => r.Name.ToLower().Contains(search.Trim().ToLower()));

        Query.OrderBy(r => r.Order).ThenBy(r => r.Name);

        SpecificationExtensions<Room>.ApplyPaging(Query, page, pageSize);
    }
}
