using Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;

namespace Edvantix.Catalog.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация <see cref="ITimezoneRepository"/>.
/// Все запросы выполняются с <c>AsNoTracking</c> — данные read-only.
/// </summary>
public sealed class TimezoneRepository(CatalogDbContext dbContext) : ITimezoneRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => dbContext;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Timezone>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.Timezones.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(t => t.IsActive);
        }

        // Сортировка по смещению UTC, затем по коду для детерминированного порядка
        return await query
            .OrderBy(t => t.UtcOffsetMinutes)
            .ThenBy(t => t.Code)
            .ToListAsync(cancellationToken);
    }
}
