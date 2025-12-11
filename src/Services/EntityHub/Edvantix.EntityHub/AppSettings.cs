using System.Diagnostics.CodeAnalysis;

namespace Edvantix.EntityHub;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
