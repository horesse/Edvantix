using Edvantix.Catalog.Grpc.Services;

namespace Edvantix.Catalog.Grpc.Mapping;

/// <summary>
/// Маппер <see cref="CurrencyModel"/> → <see cref="CurrencyResponse"/>.
/// </summary>
internal sealed class CurrencyProtoMapper : Mapper<CurrencyModel, CurrencyResponse>
{
    /// <inheritdoc/>
    public override CurrencyResponse Map(CurrencyModel source) =>
        new()
        {
            Code = source.Code,
            NumericCode = source.NumericCode,
            NameRu = source.NameRu,
            NameEn = source.NameEn,
            Symbol = source.Symbol,
            DecimalDigits = source.DecimalDigits,
            IsActive = source.IsActive,
        };
}
