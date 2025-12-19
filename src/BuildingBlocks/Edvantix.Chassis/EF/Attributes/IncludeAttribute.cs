namespace Edvantix.Chassis.EF.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class IncludeAttribute(bool thenInclude = true) : Attribute
{
    public bool ThenInclude => thenInclude;
}
