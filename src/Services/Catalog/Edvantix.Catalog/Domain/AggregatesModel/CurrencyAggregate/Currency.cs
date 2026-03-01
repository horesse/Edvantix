namespace Edvantix.Catalog.Domain.AggregatesModel.CurrencyAggregate;

/// <summary>
/// Валюта по стандарту ISO 4217. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Currency() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую запись валюты.
    /// </summary>
    /// <param name="code">Алфавитный код ISO 4217 (например, "USD").</param>
    /// <param name="name">Наименование валюты (например, "US Dollar").</param>
    /// <param name="symbol">Символ валюты (например, "$").</param>
    /// <param name="numericCode">Числовой код ISO 4217 (например, 840).</param>
    /// <param name="decimalDigits">Количество десятичных знаков для денежных сумм.</param>
    public Currency(string code, string name, string symbol, int numericCode, int decimalDigits)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        Code = code;
        Name = name;
        Symbol = symbol;
        NumericCode = numericCode;
        DecimalDigits = decimalDigits;
        IsActive = true;
    }

    /// <summary>Алфавитный код ISO 4217 (например, "USD", "EUR").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование валюты (например, "US Dollar").</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Символ валюты (например, "$", "€").</summary>
    public string Symbol { get; private set; } = null!;

    /// <summary>Числовой код ISO 4217 (например, 840 для USD).</summary>
    public int NumericCode { get; private set; }

    /// <summary>Количество десятичных знаков для денежных сумм (например, 2 для USD).</summary>
    public int DecimalDigits { get; private set; }

    /// <summary>Признак активности валюты в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля валюты.
    /// </summary>
    /// <param name="name">Новое наименование.</param>
    /// <param name="symbol">Новый символ.</param>
    /// <param name="isActive">Новый статус активности.</param>
    public void Update(string name, string symbol, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        Name = name;
        Symbol = symbol;
        IsActive = isActive;
    }
}
