using Edvantix.Catalog.Grpc.Services;

namespace Edvantix.Catalog.Grpc.Mapping;

/// <summary>
/// Маппер <see cref="CountryModel"/> → <see cref="CountryResponse"/>.
/// </summary>
internal sealed class CountryProtoMapper : Mapper<CountryModel, CountryResponse>
{
    /// <inheritdoc/>
    public override CountryResponse Map(CountryModel source) =>
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
