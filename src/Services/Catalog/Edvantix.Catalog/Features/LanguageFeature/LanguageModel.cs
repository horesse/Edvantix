namespace Edvantix.Catalog.Features.LanguageFeature;

/// <summary>
/// DTO языка для REST API ответов.
/// </summary>
public sealed class LanguageModel
{
    /// <summary>Двухбуквенный код ISO 639-1 (например, "en", "ru").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Наименование языка на русском (например, "Английский").</summary>
    public string NameRu { get; set; } = null!;

    /// <summary>Наименование языка на английском (например, "English").</summary>
    public string NameEn { get; set; } = null!;

    /// <summary>Название языка на родном языке (например, "Deutsch", "Русский").</summary>
    public string NativeName { get; set; } = null!;

    /// <summary>Признак активности языка в системе.</summary>
    public bool IsActive { get; set; }
}
