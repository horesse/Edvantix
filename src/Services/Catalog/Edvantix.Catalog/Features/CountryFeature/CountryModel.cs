namespace Edvantix.Catalog.Features.CountryFeature;

/// <summary>
/// DTO страны для REST API ответов.
/// </summary>
public sealed class CountryModel
{
    /// <summary>Двухбуквенный код ISO 3166-1 alpha-2 (например, "US", "DE").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Трёхбуквенный код ISO 3166-1 alpha-3 (например, "USA", "DEU").</summary>
    public string Alpha3Code { get; set; } = null!;

    /// <summary>Числовой код ISO 3166-1 (например, 840).</summary>
    public int NumericCode { get; set; }

    /// <summary>Наименование страны на русском языке.</summary>
    public string NameRu { get; set; } = null!;

    /// <summary>Наименование страны на английском языке.</summary>
    public string NameEn { get; set; } = null!;

    /// <summary>Международный телефонный префикс (например, "+1", "+7").</summary>
    public string PhonePrefix { get; set; } = null!;

    /// <summary>Код национальной валюты по ISO 4217 (например, "USD").</summary>
    public string CurrencyCode { get; set; } = null!;

    /// <summary>Признак активности страны в системе.</summary>
    public bool IsActive { get; set; }
}
