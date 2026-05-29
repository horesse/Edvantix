namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта способов оплаты организации.
/// <para>
/// <paramref name="isArchive"/> = <see langword="false"/> (по умолчанию) — только активные,
/// <paramref name="isArchive"/> = <see langword="true"/> — только архивные.
/// </para>
/// </summary>
public sealed class PaymentMethodCountSpecification : Specification<PaymentMethod>
{
    public PaymentMethodCountSpecification(
        Guid organizationId,
        bool isArchive = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(pm => pm.OrganizationId == organizationId);

        if (isArchive)
            Query.IgnoreQueryFilters().Where(pm => pm.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
            Query.Where(pm => pm.Name.ToLower().Contains(search.Trim().ToLower()));
    }
}
