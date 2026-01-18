using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Company.Domain.AggregatesModel.UsageAggregate.Specifications;

public sealed class UsageSpecification : CommonSpecification<Usage>
{
    private readonly long? _organizationId;
    private readonly long? _limitId;

    public long? OrganizationId
    {
        get => _organizationId;
        init
        {
            _organizationId = value;
            ApplyWhereExpressions();
        }
    }

    public long? LimitId
    {
        get => _limitId;
        init
        {
            _limitId = value;
            ApplyWhereExpressions();
        }
    }

    private void ApplyWhereExpressions()
    {
        if (_organizationId.HasValue)
            Query.Where(x => x.OrganizationId == _organizationId.Value);

        if (_limitId.HasValue)
            Query.Where(x => x.LimitId == _limitId.Value);
    }
}
