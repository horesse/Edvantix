using Edvantix.Catalog.Application.Behaviors;

namespace Edvantix.Catalog.Features.RegionFeature.GetRegionByCode;

/// <summary>
/// Запрос региона по коду. Реализует <see cref="ICachedQuery"/> — результат кэшируется через HybridCache.
/// </summary>
public sealed record GetRegionByCodeQuery(string Code) : IQuery<RegionModel>, ICachedQuery
{
    /// <inheritdoc/>
    public string CacheKey => $"catalog:region:{Code.ToUpperInvariant()}";

    /// <inheritdoc/>
    public string[] Tags => [CatalogEntityType.Region];

    /// <inheritdoc/>
    public TimeSpan? Expiry => TimeSpan.FromHours(4);
}

/// <summary>
/// Обработчик <see cref="GetRegionByCodeQuery"/>.
/// Выбрасывает <see cref="NotFoundException"/>, если регион не найден.
/// </summary>
public sealed class GetRegionByCodeQueryHandler(IServiceProvider provider)
    : IQueryHandler<GetRegionByCodeQuery, RegionModel>
{
    /// <inheritdoc/>
    public async ValueTask<RegionModel> Handle(
        GetRegionByCodeQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IRegionRepository>();
        var mapper = provider.GetRequiredService<IMapper<Region, RegionModel>>();

        var region =
            await repo.GetByCodeAsync(request.Code, cancellationToken)
            ?? throw new NotFoundException($"Регион с кодом '{request.Code}' не найден.");

        return mapper.Map(region);
    }
}
