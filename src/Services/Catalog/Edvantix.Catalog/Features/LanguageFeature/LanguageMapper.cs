namespace Edvantix.Catalog.Features.LanguageFeature;

/// <summary>
/// Маппер <see cref="Language"/> → <see cref="LanguageModel"/>.
/// </summary>
public sealed class LanguageMapper : Mapper<Language, LanguageModel>
{
    /// <inheritdoc/>
    public override LanguageModel Map(Language source) =>
        new()
        {
            Code = source.Code,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            NativeName = source.NativeName,
            IsActive = source.IsActive,
        };
}
