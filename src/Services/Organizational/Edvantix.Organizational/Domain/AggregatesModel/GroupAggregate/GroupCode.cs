using System.Text.RegularExpressions;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Value object для кода группы — уникального идентификатора в рамках организации.
/// <para>Формат: заглавные буквы, цифры и дефисы; максимум 32 символа. Пример: <c>EN-B1-12</c>.</para>
/// </summary>
public sealed record GroupCode
{
    /// <summary>Регулярное выражение допустимого формата кода.</summary>
    private static readonly Regex FormatPattern = new(
        @"^[A-Z0-9\-]+$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    private GroupCode(string value) => Value = value;

    /// <summary>Строковое значение кода группы.</summary>
    public string Value { get; }

    /// <summary>
    /// Создаёт <see cref="GroupCode"/> из строки.
    /// </summary>
    /// <param name="value">Значение кода.</param>
    /// <exception cref="ArgumentException">Если строка пуста, длиннее 32 символов или не соответствует формату.</exception>
    public static GroupCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Код группы не может быть пустым.", nameof(value));

        var trimmed = value.Trim().ToUpperInvariant();

        if (trimmed.Length > 32)
            throw new ArgumentException("Код группы не может превышать 32 символа.", nameof(value));

        if (!FormatPattern.IsMatch(trimmed))
            throw new ArgumentException(
                "Код группы должен содержать только заглавные латинские буквы, цифры и дефисы.",
                nameof(value)
            );

        return new GroupCode(trimmed);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
