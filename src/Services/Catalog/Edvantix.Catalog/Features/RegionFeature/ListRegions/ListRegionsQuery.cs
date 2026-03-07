using System.ComponentModel;
using Edvantix.Catalog.Application.Behaviors;

namespace Edvantix.Catalog.Features.RegionFeature.ListRegions;

/// <summary>
/// Запрос списка регионов. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record ListRegionsQuery(
    [property: Description("Фильтровать только активные регионы")]
    [property: DefaultValue(true)]
        bool ActiveOnly = true
) : IRequest<IReadOnlyList<RegionModel>>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:regions:list:{ActiveOnly}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Region];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(1);
}

/// <summary>
/// Обработчик <see cref="ListRegionsQuery"/>.
/// </summary>
public sealed class ListRegionsQueryHandler(IServiceProvider provider)
    : IRequestHandler<ListRegionsQuery, IReadOnlyList<RegionModel>>
{
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<RegionModel>> Handle(
        ListRegionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IRegionRepository>();
        var mapper = provider.GetRequiredService<IMapper<Region, RegionModel>>();

        var regions = await repo.ListAsync(request.ActiveOnly, cancellationToken);

        return mapper.Map(regions);
    }
}
