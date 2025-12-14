namespace Edvantix.Chassis.Utilities.Guards;

public sealed class Guard
{
    private Guard() { }

    public static Guard Against { get; } = new();
}
