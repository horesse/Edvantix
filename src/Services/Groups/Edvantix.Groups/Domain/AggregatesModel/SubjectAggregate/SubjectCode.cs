using System.Text.RegularExpressions;

namespace Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

/// <summary>
/// Value object для кода предмета — уникального идентификатора в рамках организации.
/// <para>Формат: заглавные латинские буквы и цифры; максимум 10 символов. Пример: <c>MATH</c>, <c>ENG101</c>.</para>
/// </summary>
public sealed record SubjectCode
{
    /// <summary>Регулярное выражение допустимого формата кода предмета.</summary>
    private static readonly Regex FormatPattern = new(
        @"^[A-Z0-9]{1,10}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    private SubjectCode(string value) => Value = value;

    /// <summary>Строковое значение кода предмета.</summary>
    public string Value { get; }

    /// <summary>
    /// Создаёт <see cref="SubjectCode"/> из строки.
    /// </summary>
    /// <param name="value">Значение кода.</param>
    /// <exception cref="ArgumentException">Если строка пуста, длиннее 10 символов или не соответствует формату.</exception>
    public static SubjectCode From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Код предмета не может быть пустым.", nameof(value));

        var trimmed = value.Trim().ToUpperInvariant();

        if (!FormatPattern.IsMatch(trimmed))
            throw new ArgumentException(
                "Код предмета должен содержать только заглавные латинские буквы и цифры, и не превышать 10 символов.",
                nameof(value)
            );

        return new SubjectCode(trimmed);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
