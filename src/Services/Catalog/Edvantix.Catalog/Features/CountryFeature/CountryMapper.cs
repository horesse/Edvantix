namespace Edvantix.Catalog.Features.CountryFeature;

/// <summary>
/// Маппер <see cref="Country"/> → <see cref="CountryModel"/>.
/// </summary>
public sealed class CountryMapper : Mapper<Country, CountryModel>
{
    /// <inheritdoc/>
    public override CountryModel Map(Country source) =>
        new()
        {
            Code = source.Code,
            Alpha3Code = source.Alpha3Code,
            NumericCode = source.NumericCode,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            PhonePrefix = source.PhonePrefix,
            CurrencyCode = source.CurrencyCode,
            IsActive = source.IsActive,
        };
}
