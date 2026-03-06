namespace Edvantix.Catalog.Features.TimezoneFeature;

/// <summary>
/// DTO часового пояса для REST API ответов.
/// </summary>
public sealed class TimezoneModel
{
    /// <summary>Идентификатор IANA TZ Database (например, "Europe/Moscow").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Наименование на русском языке (например, "Московское время").</summary>
    public string NameRu { get; set; } = null!;

    /// <summary>Наименование на английском языке (например, "Moscow Time").</summary>
    public string NameEn { get; set; } = null!;

    /// <summary>Отображаемое название со смещением UTC (например, "(UTC+03:00) Moscow").</summary>
    public string DisplayName { get; set; } = null!;

    /// <summary>Смещение относительно UTC в минутах (например, 180 для UTC+3).</summary>
    public int UtcOffsetMinutes { get; set; }

    /// <summary>Признак активности часового пояса в системе.</summary>
    public bool IsActive { get; set; }
}
