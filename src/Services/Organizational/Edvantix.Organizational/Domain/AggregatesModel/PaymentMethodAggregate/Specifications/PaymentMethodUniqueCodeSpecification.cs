namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация для проверки уникальности кода способа оплаты
/// среди активных записей организации.
/// </summary>
public sealed class PaymentMethodUniqueCodeSpecification : Specification<PaymentMethod>
{
    public PaymentMethodUniqueCodeSpecification(
        Guid organizationId,
        string code,
        Guid? excludeId = null
    )
    {
        Query
            .AsNoTracking()
            .Where(pm => pm.OrganizationId == organizationId && !pm.IsArchived && pm.Code == code);

        if (excludeId.HasValue)
            Query.Where(pm => pm.Id != excludeId.Value);
    }
}
