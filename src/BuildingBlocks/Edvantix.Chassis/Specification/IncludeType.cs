using System.ComponentModel;

namespace Edvantix.Chassis.Specification;

public enum IncludeType : byte
{
    [Description("Include")]
    Include = 0,

    [Description("Then Include")]
    ThenInclude = 1,
}
