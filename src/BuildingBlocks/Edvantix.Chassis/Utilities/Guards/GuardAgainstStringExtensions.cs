using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Chassis.Utilities.Guards;

public static class GuardAgainstStringExtensions
{
    extension(Guard guard)
    {
        public string NullOrWhiteSpace([NotNull] string? value, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
            return value;
        }
    }
}
