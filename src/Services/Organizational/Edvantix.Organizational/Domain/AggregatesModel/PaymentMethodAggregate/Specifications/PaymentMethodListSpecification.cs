using Edvantix.Chassis.Specification.Extensions;

namespace Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;

/// <summary>
/// Спецификация постраничного списка способов оплаты организации.
/// Поддерживает фильтрацию по архивности и поиск по названию.
/// </summary>
public sealed class PaymentMethodListSpecification : Specification<PaymentMethod>
{
    public PaymentMethodListSpecification(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize
    )
    {
        Query.AsNoTracking().Where(pm => pm.OrganizationId == organizationId);

        if (!includeArchived)
            Query.Where(pm => !pm.IsArchived);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            Query.Where(pm => pm.Name.ToLower().Contains(term));
        }

        Query.OrderBy(pm => pm.Order).ThenBy(pm => pm.Name);

        SpecificationExtensions<PaymentMethod>.ApplyPaging(Query, page, pageSize);
    }
}
