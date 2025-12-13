using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Edvantix.Chassis.EF.Attributes;
using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Expressions;
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

    public CommonSpecification()
    {
        ApplyOrderBy();
    }
    
    protected virtual void ApplyFilters()
    {
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)) && _showDeleted)
        {
            IgnoreQueryFilters = true;
        }
    }

    private void ApplyOrderBy()
    {
        if (OrderExpressions.Any())
            return;
        
        var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<OrderByAttribute>() != null)
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetCustomAttribute<OrderByAttribute>()!
            })
            .OrderBy(x => x.Attribute.Order)
            .ToList();

        foreach (var item in properties)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, item.Property);
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<TEntity, object?>>(conversion, parameter);
            
            Query.Order(lambda, item.Attribute.Order);
        }
    }
}
