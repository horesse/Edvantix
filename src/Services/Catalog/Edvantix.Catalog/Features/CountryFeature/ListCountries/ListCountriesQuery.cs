using System.ComponentModel;
using Edvantix.Catalog.Application.Behaviors;
using Edvantix.Catalog.Features.CountryFeature;

namespace Edvantix.Catalog.Features.CountryFeature.ListCountries;

/// <summary>
/// Запрос списка стран. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record ListCountriesQuery(
    [property: Description("Фильтровать только активные страны")]
    [property: DefaultValue(true)]
        bool ActiveOnly = true
) : IRequest<IReadOnlyList<CountryModel>>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:countries:list:{ActiveOnly}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Country];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(1);
}

/// <summary>
/// Обработчик <see cref="ListCountriesQuery"/>.
/// </summary>
public sealed class ListCountriesQueryHandler(IServiceProvider provider)
    : IRequestHandler<ListCountriesQuery, IReadOnlyList<CountryModel>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<CountryModel>> Handle(
        ListCountriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICountryRepository>();
        var mapper = provider.GetRequiredService<IMapper<Country, CountryModel>>();

        var countries = await repo.ListAsync(request.ActiveOnly, cancellationToken);

        return mapper.Map(countries);
    }
}
