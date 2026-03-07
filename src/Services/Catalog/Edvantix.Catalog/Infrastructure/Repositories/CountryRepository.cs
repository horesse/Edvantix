namespace Edvantix.Catalog.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация <see cref="ICountryRepository"/>.
/// Все запросы выполняются с <c>AsNoTracking</c> — данные read-only.
/// </summary>
public sealed class CountryRepository(CatalogDbContext dbContext) : ICountryRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Country>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Countries.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderBy(c => c.Code).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Country?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext
            .Countries.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Country?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext.Countries.FirstOrDefaultAsync(
            c => c.Code == normalized,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task AddAsync(Country entity, CancellationToken cancellationToken = default) =>
        await dbContext.Countries.AddAsync(entity, cancellationToken);
}
