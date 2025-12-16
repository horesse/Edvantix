using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Edvantix.Chassis.EF.Attributes;
using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Expressions;

namespace Edvantix.Chassis.Specification.Generic;

public class AttributeSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    protected AttributeSpecification()
    {
        ApplyIncludeAttributes();
        ApplyOrderByAttribute();
    }

    private void ApplyIncludeAttributes()
    {
        if (IncludeExpressions.Any() || IncludeStrings.Any())
            return;

        ApplyIncludesRecursive(
            entityType: typeof(TEntity),
            parameter: Expression.Parameter(typeof(TEntity), "x"),
            previousPropertyType: null
        );
    }

    private void ApplyIncludesRecursive(
        Type entityType,
        ParameterExpression parameter,
        Type? previousPropertyType
    )
    {
        var properties = entityType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<IncludeAttribute>() != null)
            .ToList();

        foreach (var property in properties)
        {
            var propertyExpression = Expression.Property(parameter, property);
            var lambda = Expression.Lambda(propertyExpression, parameter);

            if (previousPropertyType is null)
            {
                var expression = new IncludeExpression(lambda);
                Add(expression);
            }
            else
            {
                var expression = new IncludeExpression(lambda, previousPropertyType);
                Add(expression);
            }

            var navigationType = GetNavigationType(property.PropertyType);
            if (navigationType is null)
                continue;

            // Рекурсия для вложенных Include
            var nestedParameter = Expression.Parameter(navigationType, "x");

            ApplyIncludesRecursive(
                entityType: navigationType,
                parameter: nestedParameter,
                previousPropertyType: property.PropertyType
            );
        }
    }

    private static Type? GetNavigationType(Type propertyType)
    {
        if (propertyType == typeof(string))
            return null;

        if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType.IsGenericType)
        {
            return propertyType.GetGenericArguments()[0];
        }

        return propertyType.IsClass ? propertyType : null;
    }

    private void ApplyOrderByAttribute()
    {
        if (OrderExpressions.Any())
            return;

        var properties = typeof(TEntity)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<OrderByAttribute>() != null)
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetCustomAttribute<OrderByAttribute>()!,
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
