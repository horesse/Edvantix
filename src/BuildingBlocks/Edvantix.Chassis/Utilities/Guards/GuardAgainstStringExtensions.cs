using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Chassis.Utilities.Guards;

/// <summary>
/// Guard extensions for string validation.
/// </summary>
public static class GuardAgainstStringExtensions
{
    extension(Guard guard)
    {
        /// <summary>
        /// Validates that the provided string is not null or whitespace.
        /// </summary>
        /// <param name="value">The string to validate.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The trimmed string value if validation passes.</returns>
        /// <exception cref="ArgumentException">Thrown when the value is null, empty, or whitespace.</exception>
        public string NullOrWhiteSpace([NotNull] string? value, string parameterName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
            return value;
        }

        /// <summary>
        /// Validates that the provided value is not the default value for its type.
        /// </summary>
        /// <typeparam name="T">The type of the value being validated.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The value if validation passes.</returns>
        /// <exception cref="ArgumentException">Thrown when the value equals the default for its type.</exception>
        public T Default<T>(T value, string parameterName)
            where T : struct, IEquatable<T>
        {
            if (value.Equals(default(T)))
            {
                throw new ArgumentException(
                    $"Parameter [{parameterName}] is default value for type {typeof(T).Name}.",
                    parameterName
                );
            }

            return value;
        }
    }
}
