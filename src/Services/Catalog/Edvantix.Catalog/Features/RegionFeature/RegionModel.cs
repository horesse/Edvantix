namespace Edvantix.Catalog.Features.RegionFeature;

/// <summary>
/// DTO региона для REST API ответов.
/// </summary>
public sealed class RegionModel
{
    /// <summary>Код региона, натуральный PK (например, "CIS", "EU", "APAC").</summary>
    public string Code { get; set; } = null!;

    /// <summary>Наименование региона на русском (например, "СНГ").</summary>
    public string NameRu { get; set; } = null!;

    /// <summary>Наименование региона на английском (например, "CIS").</summary>
    public string NameEn { get; set; } = null!;

    /// <summary>Признак активности региона в системе.</summary>
    public bool IsActive { get; set; }
}
