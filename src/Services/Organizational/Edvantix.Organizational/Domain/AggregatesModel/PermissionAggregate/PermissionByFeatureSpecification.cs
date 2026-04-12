using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public sealed class PermissionByFeatureSpecification : Specification<Permission>
{
    public PermissionByFeatureSpecification(string feature)
    {
        Query.Where(p => p.Feature == feature).OrderBy(p => p.Name);
    }
}
