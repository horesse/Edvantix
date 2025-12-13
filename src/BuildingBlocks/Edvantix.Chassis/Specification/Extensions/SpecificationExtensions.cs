using Edvantix.Chassis.Specification.Builders;

namespace Edvantix.Chassis.Specification.Extensions;

public static class SpecificationExtensions<TEntity>
    where TEntity : class
{
    public static void ApplyPaging(
        ISpecificationBuilder<TEntity> builder,
        int pageIndex,
        int pageSize
    )
    {
        builder.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }
}
