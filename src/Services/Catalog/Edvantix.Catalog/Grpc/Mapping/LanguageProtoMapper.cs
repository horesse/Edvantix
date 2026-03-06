using Edvantix.Catalog.Grpc.Services;

namespace Edvantix.Catalog.Grpc.Mapping;

/// <summary>
/// Маппер <see cref="LanguageModel"/> → <see cref="LanguageResponse"/>.
/// </summary>
internal sealed class LanguageProtoMapper : Mapper<LanguageModel, LanguageResponse>
{
    /// <inheritdoc/>
    public override LanguageResponse Map(LanguageModel source) =>
        new()
        {
            Code = source.Code,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            NativeName = source.NativeName,
            IsActive = source.IsActive,
        };
}
