namespace Edvantix.Catalog.Domain.AggregatesModel.CountryAggregate;

/// <summary>
/// Страна по стандарту ISO 3166-1. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Country : HasDomainEvents, IAggregateRoot
{
    /// <summary>EF Core требует параметризованный или безпараметровый конструктор для материализации.</summary>
    private Country() { }

    /// <summary>
    /// Создаёт новую запись страны и публикует событие <see cref="CatalogChangeType.Created"/>.
    /// </summary>
    /// <param name="code">Двухбуквенный код ISO 3166-1 alpha-2 (например, "US"). Натуральный PK.</param>
    /// <param name="alpha3Code">Трёхбуквенный код ISO 3166-1 alpha-3 (например, "USA").</param>
    /// <param name="nameRu">Наименование страны на русском языке.</param>
    /// <param name="nameEn">Наименование страны на английском языке.</param>
    /// <param name="numericCode">Числовой код ISO 3166-1 (например, 840).</param>
    /// <param name="phonePrefix">Международный телефонный префикс (например, "+1").</param>
    /// <param name="currencyCode">Код национальной валюты ISO 4217 (например, "USD").</param>
    public Country(
        string code,
        string alpha3Code,
        string nameRu,
        string nameEn,
        int numericCode,
        string phonePrefix,
        string currencyCode
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(alpha3Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(phonePrefix);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        Code = code.ToUpperInvariant();
        Alpha3Code = alpha3Code.ToUpperInvariant();
        NameRu = nameRu;
        NameEn = nameEn;
        NumericCode = numericCode;
        PhonePrefix = phonePrefix;
        CurrencyCode = currencyCode.ToUpperInvariant();
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(CatalogEntityType.Country, Code, CatalogChangeType.Created)
        );
    }

    /// <summary>Двухбуквенный код ISO 3166-1 alpha-2, натуральный PK (например, "US", "DE").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Трёхбуквенный код ISO 3166-1 alpha-3 (например, "USA", "DEU").</summary>
    public string Alpha3Code { get; private set; } = null!;

    /// <summary>Наименование страны на русском (например, "Соединённые Штаты Америки").</summary>
    public string NameRu { get; private set; } = null!;

    /// <summary>Наименование страны на английском (например, "United States").</summary>
    public string NameEn { get; private set; } = null!;

    /// <summary>Числовой код ISO 3166-1 (например, 840 для США).</summary>
    public int NumericCode { get; private set; }

    /// <summary>Международный телефонный префикс (например, "+1", "+7").</summary>
    public string PhonePrefix { get; private set; } = null!;

    /// <summary>Код национальной валюты по ISO 4217 (FK на <see cref="Currency.Code"/>).</summary>
    public string CurrencyCode { get; private set; } = null!;

    /// <summary>Признак активности страны в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля страны и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    /// <param name="nameRu">Новое наименование на русском.</param>
    /// <param name="nameEn">Новое наименование на английском.</param>
    /// <param name="phonePrefix">Новый телефонный префикс.</param>
    /// <param name="currencyCode">Новый код валюты.</param>
    public void Update(string nameRu, string nameEn, string phonePrefix, string currencyCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(phonePrefix);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        NameRu = nameRu;
        NameEn = nameEn;
        PhonePrefix = phonePrefix;
        CurrencyCode = currencyCode.ToUpperInvariant();

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(CatalogEntityType.Country, Code, CatalogChangeType.Updated)
        );
    }

    /// <summary>
    /// Деактивирует страну и публикует событие <see cref="CatalogChangeType.Deactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Country,
                Code,
                CatalogChangeType.Deactivated
            )
        );
    }
}
