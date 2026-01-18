using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Company;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
