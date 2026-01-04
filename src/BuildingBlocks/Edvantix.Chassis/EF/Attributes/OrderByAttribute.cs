using Edvantix.Chassis.Specification;

namespace Edvantix.Chassis.EF.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OrderByAttribute(OrderType order = OrderType.OrderBy, int priority = 1)
    : Attribute
{
    public OrderType Order => order;

    public int Priority => order is OrderType.OrderBy or OrderType.OrderByDescending ? 0 : priority;
}
