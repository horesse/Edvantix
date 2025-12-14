using Edvantix.Chassis.Specification;

namespace Edvantix.Chassis.EF.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class OrderByAttribute(OrderType order = OrderType.OrderBy) : Attribute
{
    public OrderType Order => order;
}
