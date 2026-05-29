using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка способов оплаты организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — активные записи.
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные (удалённые).
/// </para>
/// </summary>
public sealed class PaymentMethodListSpecification : Specification<PaymentMethod>
{
    public PaymentMethodListSpecification(
        Guid organizationId,
        bool isArchive,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(pm => pm.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(pm => pm.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(pm => pm.Name.ToLower().Contains(search.Trim().ToLower()));

        Query.OrderBy(pm => pm.Order).ThenBy(pm => pm.Name);

        SpecificationExtensions<PaymentMethod>.ApplyPaging(Query, page, pageSize);
    }
}
