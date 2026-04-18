namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public sealed class PermissionSpecification : Specification<Permission>
{
    public PermissionSpecification()
    {
        Query.OrderBy(x => x.Name);
    }
}
