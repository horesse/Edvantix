namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности названия способа оплаты
/// среди активных записей организации.
/// </summary>
public sealed class PaymentMethodUniqueNameSpecification : Specification<PaymentMethod>
{
    public PaymentMethodUniqueNameSpecification(
        Guid organizationId,
        string name,
        Guid? excludeId = null
    )
    {
        Query.AsNoTracking().Where(pm => pm.OrganizationId == organizationId && pm.Name == name);

        if (excludeId.HasValue)
            Query.Where(pm => pm.Id != excludeId.Value);
    }
}
