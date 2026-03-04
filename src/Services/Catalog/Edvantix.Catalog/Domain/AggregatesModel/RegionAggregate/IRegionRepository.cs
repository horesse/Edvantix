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

    /// <summary>Возвращает регион по коду без трекинга или <c>null</c>.</summary>
    Task<Region?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Возвращает отслеживаемую EF Core сущность для операций записи или <c>null</c>.</summary>
    Task<Region?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новый регион в контекст EF Core.</summary>
    Task AddAsync(Region entity, CancellationToken cancellationToken = default);
}
