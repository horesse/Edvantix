using System.ComponentModel;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Specification.Generic;

public class CommonSpecification<TEntity> : Specification<TEntity>
    where TEntity : class, IAggregateRoot
{
    private readonly bool _showDeleted;

    [DefaultValue(false)]
    public bool ShowDeleted
    {
        get => _showDeleted;
        init
        {
            _showDeleted = value;
            ApplyFilters();
        }
    }

    protected virtual void ApplyFilters()
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)) && _showDeleted)
        {
            IgnoreQueryFilters = true;
        }
    }
}
