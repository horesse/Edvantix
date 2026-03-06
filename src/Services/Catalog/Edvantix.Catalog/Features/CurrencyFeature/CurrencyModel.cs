namespace Edvantix.Catalog.Features.CurrencyFeature;

/// <summary>
/// DTO валюты для REST API ответов.
/// </summary>
public sealed class CurrencyModel
{
    /// <summary>Алфавитный код ISO 4217 (например, "USD", "EUR").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Числовой код ISO 4217 (например, 840).</summary>
    public int NumericCode { get; set; }

    /// <summary>Наименование на русском языке.</summary>
    public string NameRu { get; set; } = null!;

    /// <summary>Наименование на английском языке.</summary>
    public string NameEn { get; set; } = null!;

    /// <summary>Символ валюты (например, "$", "€").</summary>
    public string Symbol { get; set; } = null!;

    /// <summary>Количество десятичных знаков для денежных сумм.</summary>
    public int DecimalDigits { get; set; }

    /// <summary>Признак активности валюты в системе.</summary>
    public bool IsActive { get; set; }
}
