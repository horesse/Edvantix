using Edvantix.Catalog.Application.Behaviors;

namespace Edvantix.Catalog.Features.LanguageFeature.GetLanguageByCode;

/// <summary>
/// Запрос языка по ISO 639-1-коду. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record GetLanguageByCodeQuery(string Code) : IQuery<LanguageModel>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:language:{Code.ToLowerInvariant()}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Language];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(4);
}

/// <summary>
/// Обработчик <see cref="GetLanguageByCodeQuery"/>.
/// Выбрасывает <see cref="NotFoundException"/>, если язык не найден.
/// </summary>
public sealed class GetLanguageByCodeQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetLanguageByCodeQuery, LanguageModel>
{
    /// <inheritdoc/>
    public async ValueTask<LanguageModel> Handle(
        GetLanguageByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ILanguageRepository>();
        var mapper = provider.GetRequiredService<IMapper<Language, LanguageModel>>();

        var language =
            await repo.GetByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Язык с кодом '{request.Code}' не найден.");

        return mapper.Map(language);
    }
}
