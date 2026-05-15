using System.Text.RegularExpressions;

namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

/// <summary>
/// Value object для кода уровня — уникального идентификатора в рамках организации.
/// <para>Формат: заглавные буквы, цифры, дефис и подчёркивание; максимум 16 символов. Пример: <c>A1</c>, <c>B2_ADV</c>.</para>
/// </summary>
public sealed record LevelCode
{
    /// <summary>Регулярное выражение допустимого формата кода уровня.</summary>
    private static readonly Regex FormatPattern = new(
        @"^[A-Z0-9_-]{1,16}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    private LevelCode(string value) => Value = value;

    /// <summary>Строковое значение кода уровня.</summary>
    public string Value { get; }

    /// <summary>
    /// Создаёт <see cref="LevelCode"/> из строки.
    /// </summary>
    /// <param name="value">Значение кода.</param>
    /// <exception cref="ArgumentException">Если строка пуста, длиннее 16 символов или не соответствует формату.</exception>
    public static LevelCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Код уровня не может быть пустым.", nameof(value));

        var trimmed = value.Trim().ToUpperInvariant();

        if (!FormatPattern.IsMatch(trimmed))
            throw new ArgumentException(
                "Код уровня должен содержать только заглавные латинские буквы, цифры, дефисы и подчёркивания, и не превышать 16 символов.",
                nameof(value)
            );

        return new LevelCode(trimmed);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
