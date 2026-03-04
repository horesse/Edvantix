using Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;

namespace Edvantix.Catalog.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация <see cref="ILanguageRepository"/>.
/// Все запросы выполняются с <c>AsNoTracking</c> — данные read-only.
/// </summary>
public sealed class LanguageRepository(CatalogDbContext dbContext) : ILanguageRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Language>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Languages.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(l => l.IsActive);
        }

        return await query.OrderBy(l => l.Code).ToListAsync(cancellationToken);
    }
}
