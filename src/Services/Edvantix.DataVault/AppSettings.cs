using System.Diagnostics.CodeAnalysis;

namespace Edvantix.DataVault;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
