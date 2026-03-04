using Edvantix.Catalog.Domain.AggregatesModel.RegionAggregate;

namespace Edvantix.Catalog.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация <see cref="IRegionRepository"/>.
/// Все запросы выполняются с <c>AsNoTracking</c> — данные read-only.
/// </summary>
public sealed class RegionRepository(CatalogDbContext dbContext) : IRegionRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Region>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Regions.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(r => r.IsActive);
        }

        return await query.OrderBy(r => r.Code).ToListAsync(cancellationToken);
    }
}
