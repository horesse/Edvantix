using System.ComponentModel;
using Edvantix.Catalog.Application.Behaviors;
using Edvantix.Catalog.Features.CurrencyFeature;

namespace Edvantix.Catalog.Features.CurrencyFeature.ListCurrencies;

/// <summary>
/// Запрос списка валют. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache
/// и инвалидируется при изменении любой валюты (<see cref="CatalogEntityType.Currency"/>).
/// </summary>
public sealed record ListCurrenciesQuery(
    [property: Description("Фильтровать только активные валюты")]
    [property: DefaultValue(true)]
        bool ActiveOnly = true
) : IRequest<IReadOnlyList<CurrencyModel>>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:currencies:list:{ActiveOnly}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Currency];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(1);
}

/// <summary>
/// Обработчик <see cref="ListCurrenciesQuery"/>.
/// Возвращает список валют из БД; при cache-hit вызов пропускается <see cref="CachingBehavior{TMessage,TResponse}"/>.
/// </summary>
public sealed class ListCurrenciesQueryHandler(IServiceProvider provider)
    : IRequestHandler<ListCurrenciesQuery, IReadOnlyList<CurrencyModel>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<CurrencyModel>> Handle(
        ListCurrenciesQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICurrencyRepository>();
        var mapper = provider.GetRequiredService<IMapper<Currency, CurrencyModel>>();

        var currencies = await repo.ListAsync(request.ActiveOnly, cancellationToken);

        return mapper.Map(currencies);
    }
}
