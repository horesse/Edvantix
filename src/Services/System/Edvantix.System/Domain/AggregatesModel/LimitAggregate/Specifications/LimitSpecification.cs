using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.System.Domain.AggregatesModel.LimitAggregate.Specifications;

public sealed class LimitSpecification : Specification<Limit>
{
    public LimitSpecification(long subId, LimitType type)
    {
        Query.Where(x => x.SubscriptionId == subId && x.Type == type);
    }
}
