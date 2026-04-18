namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public sealed class PermissionByFeatureSpecification : Specification<Permission>
{
    public PermissionByFeatureSpecification(string feature, bool trackWith = false)
    {
        Query.Where(p => p.Feature == feature).OrderBy(p => p.Name);

        if (trackWith)
            Query.AsTracking();
    }
}
