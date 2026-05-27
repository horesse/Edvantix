namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>Спецификация для загрузки активных способов оплаты организации с отслеживанием изменений.</summary>
public sealed class PaymentMethodReorderSpec : Specification<PaymentMethod>
{
    public PaymentMethodReorderSpec(Guid organizationId)
    {
        Query
            .Where(pm => pm.OrganizationId == organizationId && !pm.IsArchived)
            .OrderBy(pm => pm.Order)
            .AsTracking();
    }
}
