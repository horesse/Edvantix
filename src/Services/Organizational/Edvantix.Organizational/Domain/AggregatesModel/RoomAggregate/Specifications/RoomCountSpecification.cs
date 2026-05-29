namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта кабинетов организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные,
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные.
/// </para>
/// </summary>
public sealed class RoomCountSpecification : Specification<Room>
{
    public RoomCountSpecification(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(r => r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(r => r.Name.ToLower().Contains(search.Trim().ToLower()));
    }
}
