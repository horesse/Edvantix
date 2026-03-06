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

    /// <inheritdoc/>
    public async Task<Region?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext
            .Regions.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Region?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext.Regions.FirstOrDefaultAsync(
            r => r.Code == normalized,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task AddAsync(Region entity, CancellationToken cancellationToken = default) =>
        await dbContext.Regions.AddAsync(entity, cancellationToken);
}
