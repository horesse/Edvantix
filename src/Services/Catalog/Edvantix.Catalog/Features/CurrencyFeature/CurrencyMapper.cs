namespace Edvantix.Catalog.Features.CurrencyFeature;

/// <summary>
/// Маппер <see cref="Currency"/> → <see cref="CurrencyModel"/>.
/// </summary>
public sealed class CurrencyMapper : Mapper<Currency, CurrencyModel>
{
    /// <inheritdoc/>
    public override CurrencyModel Map(Currency source) =>
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
