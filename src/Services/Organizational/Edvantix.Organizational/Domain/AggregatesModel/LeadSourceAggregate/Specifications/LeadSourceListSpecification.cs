using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка источников привлечения организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи
/// (глобальный query-фильтр по <c>IsDeleted</c> применяется автоматически).
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые) записи
/// (query-фильтр игнорируется, дополнительно фильтруется по <c>IsDeleted = true</c>).
/// </para>
/// </summary>
public sealed class LeadSourceListSpecification : Specification<LeadSource>
{
    public LeadSourceListSpecification(
        Guid organizationId,
        bool isArchive,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(ls => ls.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(ls => ls.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(ls => ls.Name.ToLower().Contains(search.Trim().ToLower()));

        Query.OrderBy(ls => ls.Order).ThenBy(ls => ls.Name);

        SpecificationExtensions<LeadSource>.ApplyPaging(Query, page, pageSize);
    }
}
