using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;

public sealed class EntityTypeFilterExpression : Specification<EntityType>
{
    public EntityTypeFilterExpression(long? microserviceId = null, string? name = null)
    {
        if (microserviceId.HasValue)
            Query.Where(f => f.MicroserviceId == microserviceId);

        if (name != null)
            Query.Where(f => f.Name == name);
    }
}
