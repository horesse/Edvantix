using Edvantix.Catalog.Grpc.Services;

namespace Edvantix.Catalog.Grpc.Mapping;

/// <summary>
/// Маппер <see cref="RegionModel"/> → <see cref="RegionResponse"/>.
/// </summary>
internal sealed class RegionProtoMapper : Mapper<RegionModel, RegionResponse>
{
    /// <inheritdoc/>
    public override RegionResponse Map(RegionModel source) =>
        new()
        {
            Code = source.Code,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            IsActive = source.IsActive,
        };
}
