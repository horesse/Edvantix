namespace Edvantix.Catalog.Domain.AggregatesModel.CountryAggregate;

/// <summary>
/// Страна по стандарту ISO 3166-1. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Country() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую запись страны.
    /// </summary>
    /// <param name="alpha2Code">Двухбуквенный код ISO 3166-1 alpha-2 (например, "US").</param>
    /// <param name="alpha3Code">Трёхбуквенный код ISO 3166-1 alpha-3 (например, "USA").</param>
    /// <param name="name">Официальное наименование страны.</param>
    /// <param name="numericCode">Числовой код ISO 3166-1 (например, 840).</param>
    /// <param name="currencyCode">Код национальной валюты ISO 4217 (например, "USD").</param>
    public Country(
        string alpha2Code,
        string alpha3Code,
        string name,
        int numericCode,
        string currencyCode
    )
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alpha2Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(alpha3Code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        Alpha2Code = alpha2Code;
        Alpha3Code = alpha3Code;
        Name = name;
        NumericCode = numericCode;
        CurrencyCode = currencyCode;
        IsActive = true;
    }

    /// <summary>Двухбуквенный код ISO 3166-1 alpha-2 (например, "US", "DE").</summary>
    public string Alpha2Code { get; private set; } = null!;

    /// <summary>Трёхбуквенный код ISO 3166-1 alpha-3 (например, "USA", "DEU").</summary>
    public string Alpha3Code { get; private set; } = null!;

    /// <summary>Официальное наименование страны.</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Числовой код ISO 3166-1 (например, 840 для США).</summary>
    public int NumericCode { get; private set; }

    /// <summary>Код национальной валюты по ISO 4217 (например, "USD").</summary>
    public string CurrencyCode { get; private set; } = null!;

    /// <summary>Признак активности страны в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля страны.
    /// </summary>
    /// <param name="name">Новое наименование.</param>
    /// <param name="currencyCode">Новый код валюты.</param>
    /// <param name="isActive">Новый статус активности.</param>
    public void Update(string name, string currencyCode, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(currencyCode);

        Name = name;
        CurrencyCode = currencyCode;
        IsActive = isActive;
    }
}
