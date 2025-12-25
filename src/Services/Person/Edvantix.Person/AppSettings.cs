using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Person;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
