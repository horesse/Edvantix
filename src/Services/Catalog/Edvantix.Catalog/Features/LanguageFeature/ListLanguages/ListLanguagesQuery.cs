using System.ComponentModel;
using Edvantix.Catalog.Application.Behaviors;
using Edvantix.Catalog.Features.LanguageFeature;

namespace Edvantix.Catalog.Features.LanguageFeature.ListLanguages;

/// <summary>
/// Запрос списка языков. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record ListLanguagesQuery(
    [property: Description("Фильтровать только активные языки")]
    [property: DefaultValue(true)]
        bool ActiveOnly = true
) : IRequest<IReadOnlyList<LanguageModel>>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:languages:list:{ActiveOnly}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Language];
}

/// <summary>
/// Обработчик <see cref="ListLanguagesQuery"/>.
/// </summary>
public sealed class ListLanguagesQueryHandler(IServiceProvider provider)
    : IRequestHandler<ListLanguagesQuery, IReadOnlyList<LanguageModel>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<LanguageModel>> Handle(
        ListLanguagesQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ILanguageRepository>();
        var mapper = provider.GetRequiredService<IMapper<Language, LanguageModel>>();

        var languages = await repo.ListAsync(request.ActiveOnly, cancellationToken);

        return mapper.Map(languages);
    }
}
