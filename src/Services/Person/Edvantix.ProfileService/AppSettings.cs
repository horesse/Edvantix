using System.Diagnostics.CodeAnalysis;

namespace Edvantix.ProfileService;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
