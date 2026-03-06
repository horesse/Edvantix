namespace Edvantix.Catalog.Features.TimezoneFeature;

/// <summary>
/// Маппер <see cref="Timezone"/> → <see cref="TimezoneModel"/>.
/// </summary>
public sealed class TimezoneMapper : Mapper<Timezone, TimezoneModel>
{
    /// <inheritdoc/>
    public override TimezoneModel Map(Timezone source) =>
        new()
        {
            Code = source.Code,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            DisplayName = source.DisplayName,
            UtcOffsetMinutes = source.UtcOffsetMinutes,
            IsActive = source.IsActive,
        };
}
