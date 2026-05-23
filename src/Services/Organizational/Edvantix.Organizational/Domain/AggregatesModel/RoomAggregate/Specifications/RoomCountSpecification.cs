namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта кабинетов организации.
/// <para>
/// <paramref name="isArchived"/> = <see langword="false"/> — только активные,
/// <paramref name="isArchived"/> = <see langword="true"/> — только архивные,
/// <paramref name="isArchived"/> = <see langword="null"/> — все записи.
/// </para>
/// </summary>
public sealed class RoomCountSpecification : Specification<Room>
{
    public RoomCountSpecification(
        Guid organizationId,
        bool? isArchived = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(r => r.OrganizationId == organizationId);

        if (isArchived.HasValue)
            Query.Where(r => r.IsArchived == isArchived.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(r => r.Name.ToLower().Contains(term));
        }
    }
}
