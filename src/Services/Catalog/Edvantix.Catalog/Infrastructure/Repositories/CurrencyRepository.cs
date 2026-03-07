namespace Edvantix.Catalog.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация <see cref="ICurrencyRepository"/>.
/// Все запросы выполняются с <c>AsNoTracking</c> — данные read-only.
/// </summary>
public sealed class CurrencyRepository(CatalogDbContext dbContext) : ICurrencyRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Currency>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Currencies.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderBy(c => c.Code).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Currency?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext
            .Currencies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == normalized, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Currency?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = code.ToUpperInvariant();

        return await dbContext.Currencies.FirstOrDefaultAsync(
            c => c.Code == normalized,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task AddAsync(Currency entity, CancellationToken cancellationToken = default) =>
        await dbContext.Currencies.AddAsync(entity, cancellationToken);
}
