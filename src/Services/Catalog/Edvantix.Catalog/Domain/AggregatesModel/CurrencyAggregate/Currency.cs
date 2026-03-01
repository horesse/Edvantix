namespace Edvantix.Catalog.Domain.AggregatesModel.CurrencyAggregate;

/// <summary>
/// Валюта по стандарту ISO 4217. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Currency : HasDomainEvents, IAggregateRoot
{
    /// <summary>EF Core требует параметризованный или безпараметровый конструктор для материализации.</summary>
    private Currency() { }

    /// <summary>
    /// Создаёт новую запись валюты и публикует событие <see cref="CatalogChangeType.Created"/>.
    /// </summary>
    /// <param name="code">Алфавитный код ISO 4217 (например, "USD"). Станет натуральным PK.</param>
    /// <param name="nameRu">Наименование на русском языке.</param>
    /// <param name="nameEn">Наименование на английском языке.</param>
    /// <param name="symbol">Символ валюты (например, "$").</param>
    /// <param name="numericCode">Числовой код ISO 4217 (например, 840).</param>
    /// <param name="decimalDigits">Количество десятичных знаков для денежных сумм.</param>
    public Currency(
        string code,
        string nameRu,
        string nameEn,
        string symbol,
        int numericCode,
        int decimalDigits
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        Code = code.ToUpperInvariant();
        NameRu = nameRu;
        NameEn = nameEn;
        Symbol = symbol;
        NumericCode = numericCode;
        DecimalDigits = decimalDigits;
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Currency,
                Code,
                CatalogChangeType.Created
            )
        );
    }

    /// <summary>Алфавитный код ISO 4217, натуральный PK (например, "USD", "EUR").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование валюты на русском (например, "Доллар США").</summary>
    public string NameRu { get; private set; } = null!;

    /// <summary>Наименование валюты на английском (например, "US Dollar").</summary>
    public string NameEn { get; private set; } = null!;

    /// <summary>Символ валюты (например, "$", "€").</summary>
    public string Symbol { get; private set; } = null!;

    /// <summary>Числовой код ISO 4217 (например, 840 для USD).</summary>
    public int NumericCode { get; private set; }

    /// <summary>Количество десятичных знаков для денежных сумм (например, 2 для USD).</summary>
    public int DecimalDigits { get; private set; }

    /// <summary>Признак активности валюты в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля валюты и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    /// <param name="nameRu">Новое наименование на русском.</param>
    /// <param name="nameEn">Новое наименование на английском.</param>
    /// <param name="symbol">Новый символ.</param>
    public void Update(string nameRu, string nameEn, string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        NameRu = nameRu;
        NameEn = nameEn;
        Symbol = symbol;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Currency,
                Code,
                CatalogChangeType.Updated
            )
        );
    }

    /// <summary>
    /// Деактивирует валюту и публикует событие <see cref="CatalogChangeType.Deactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Currency,
                Code,
                CatalogChangeType.Deactivated
            )
        );
    }
}
