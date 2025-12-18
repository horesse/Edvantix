using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Organization;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
