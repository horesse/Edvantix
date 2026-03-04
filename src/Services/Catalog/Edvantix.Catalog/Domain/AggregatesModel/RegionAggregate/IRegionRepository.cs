namespace Edvantix.Catalog.Domain.AggregatesModel.RegionAggregate;

/// <summary>
/// Репозиторий для чтения справочника регионов.
/// </summary>
public interface IRegionRepository : IRepository<Region>
{
    /// <summary>Возвращает список регионов, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Region>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );
}
