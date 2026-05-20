namespace Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;

/// <summary>Код учебной группы — уникальный идентификатор внутри организации.</summary>
public sealed record GroupCode
{
    private GroupCode(string value) => Value = value;

    public string Value { get; }

    /// <summary>Создаёт GroupCode из строки. Разрешены заглавные латинские буквы, цифры и дефисы; максимум 32 символа.</summary>
    public static GroupCode From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > 32)
            throw new ArgumentException("Код группы не может превышать 32 символа.", nameof(value));

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z0-9\-]+$"))
            throw new ArgumentException(
                "Код группы должен содержать только заглавные латинские буквы, цифры и дефисы.",
                nameof(value)
            );

        return new GroupCode(value);
    }

    public override string ToString() => Value;
}
