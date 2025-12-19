using System.Diagnostics.CodeAnalysis;

namespace Edvantix.System;

[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    public int SasExpiryHours { get; set; }
}
