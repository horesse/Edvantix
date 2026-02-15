using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;

/// <summary>
/// Спецификация для поиска групп по идентификатору организации.
/// </summary>
public sealed class GroupSpecification : CommonSpecification<Group>
{
    private readonly long? _organizationId;

    public long? OrganizationId
    {
        get => _organizationId;
        init
        {
            _organizationId = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_organizationId.HasValue)
            Query.Where(x => x.OrganizationId == _organizationId.Value);
    }
}
