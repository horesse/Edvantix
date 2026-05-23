namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация для подсчёта способов оплаты организации.
/// <para>
/// <paramref name="isArchived"/> = <see langword="false"/> — только активные,
/// <paramref name="isArchived"/> = <see langword="true"/> — только архивные,
/// <paramref name="isArchived"/> = <see langword="null"/> — все записи.
/// </para>
/// </summary>
public sealed class PaymentMethodCountSpecification : Specification<PaymentMethod>
{
    public PaymentMethodCountSpecification(
        Guid organizationId,
        bool? isArchived = false,
        string? search = null
    )
    {
        Query.AsNoTracking().Where(pm => pm.OrganizationId == organizationId);

        if (isArchived.HasValue)
            Query.Where(pm => pm.IsArchived == isArchived.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(pm => pm.Name.ToLower().Contains(term));
        }
    }
}
