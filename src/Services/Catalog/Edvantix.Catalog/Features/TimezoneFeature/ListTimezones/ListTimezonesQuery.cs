using System.ComponentModel;
using Edvantix.Catalog.Application.Behaviors;
using Edvantix.Catalog.Features.TimezoneFeature;

namespace Edvantix.Catalog.Features.TimezoneFeature.ListTimezones;

/// <summary>
/// Запрос списка часовых поясов. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record ListTimezonesQuery(
    [property: Description("Фильтровать только активные часовые пояса")]
    [property: DefaultValue(true)]
        bool ActiveOnly = true
) : IRequest<IReadOnlyList<TimezoneModel>>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:timezones:list:{ActiveOnly}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Timezone];
}

/// <summary>
/// Обработчик <see cref="ListTimezonesQuery"/>.
/// </summary>
public sealed class ListTimezonesQueryHandler(IServiceProvider provider)
    : IRequestHandler<ListTimezonesQuery, IReadOnlyList<TimezoneModel>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<TimezoneModel>> Handle(
        ListTimezonesQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<ITimezoneRepository>();
        var mapper = provider.GetRequiredService<IMapper<Timezone, TimezoneModel>>();

        var timezones = await repo.ListAsync(request.ActiveOnly, cancellationToken);

        return mapper.Map(timezones);
    }
}
