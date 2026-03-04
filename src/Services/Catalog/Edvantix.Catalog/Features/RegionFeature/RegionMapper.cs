namespace Edvantix.Catalog.Features.RegionFeature;

/// <summary>
/// Маппер <see cref="Region"/> → <see cref="RegionModel"/>.
/// </summary>
public sealed class RegionMapper : Mapper<Region, RegionModel>
{
    /// <inheritdoc/>
    public override RegionModel Map(Region source) =>
        new()
        {
            Code = source.Code,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            IsActive = source.IsActive,
        };
}
