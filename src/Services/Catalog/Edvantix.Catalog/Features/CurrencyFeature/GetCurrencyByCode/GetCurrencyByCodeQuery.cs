using Edvantix.Catalog.Application.Behaviors;

namespace Edvantix.Catalog.Features.CurrencyFeature.GetCurrencyByCode;

/// <summary>
/// Запрос валюты по ISO 4217-коду. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record GetCurrencyByCodeQuery(string Code) : IRequest<CurrencyModel>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:currency:{Code.ToUpperInvariant()}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Currency];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(4);
}

/// <summary>
/// Обработчик <see cref="GetCurrencyByCodeQuery"/>.
/// Выбрасывает <see cref="NotFoundException"/>, если валюта не найдена — преобразуется в HTTP 404.
/// </summary>
public sealed class GetCurrencyByCodeQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetCurrencyByCodeQuery, CurrencyModel>
{
    /// <inheritdoc/>
    public async ValueTask<CurrencyModel> Handle(
        GetCurrencyByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ICurrencyRepository>();
        var mapper = provider.GetRequiredService<IMapper<Currency, CurrencyModel>>();

        var currency =
            await repo.GetByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Валюта с кодом '{request.Code}' не найдена.");

        return mapper.Map(currency);
    }
}
