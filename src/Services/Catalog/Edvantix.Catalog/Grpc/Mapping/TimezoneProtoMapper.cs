using Edvantix.Catalog.Grpc.Services;

namespace Edvantix.Catalog.Grpc.Mapping;

/// <summary>
/// Маппер <see cref="TimezoneModel"/> → <see cref="TimezoneResponse"/>.
/// </summary>
internal sealed class TimezoneProtoMapper : Mapper<TimezoneModel, TimezoneResponse>
{
    /// <inheritdoc/>
    public override TimezoneResponse Map(TimezoneModel source) =>
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
