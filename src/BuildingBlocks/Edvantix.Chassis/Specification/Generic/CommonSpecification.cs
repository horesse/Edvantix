using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Specification.Generic;

public class CommonSpecification<TEntity> : Specification<TEntity>
    where TEntity : class, IAggregateRoot
{
    public bool ShowDeleted { get; set; } = false;

    public CommonSpecification()
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)) && ShowDeleted)
        {
            IgnoreQueryFilters = true;
        }
    }
}
