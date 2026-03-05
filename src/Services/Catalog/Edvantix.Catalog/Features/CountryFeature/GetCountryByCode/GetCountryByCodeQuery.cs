using Edvantix.Catalog.Application.Behaviors;
using Edvantix.Catalog.Features.CountryFeature;

namespace Edvantix.Catalog.Features.CountryFeature.GetCountryByCode;

/// <summary>
/// Запрос страны по ISO 3166-1 alpha-2 коду. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record GetCountryByCodeQuery(string Code) : IRequest<CountryModel>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:country:{Code.ToUpperInvariant()}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Country];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(4);
}

/// <summary>
/// Обработчик <see cref="GetCountryByCodeQuery"/>.
/// Выбрасывает <see cref="NotFoundException"/>, если страна не найдена — преобразуется в HTTP 404.
/// </summary>
public sealed class GetCountryByCodeQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetCountryByCodeQuery, CountryModel>
{
    /// <inheritdoc/>
    public async ValueTask<CountryModel> Handle(
        GetCountryByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICountryRepository>();
        var mapper = provider.GetRequiredService<IMapper<Country, CountryModel>>();

        var country =
            await repo.GetByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Страна с кодом '{request.Code}' не найдена.");

        return mapper.Map(country);
    }
}
