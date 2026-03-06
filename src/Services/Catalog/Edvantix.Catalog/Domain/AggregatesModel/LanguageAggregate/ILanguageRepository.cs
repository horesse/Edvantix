namespace Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;

/// <summary>
/// Репозиторий для чтения справочника языков.
/// </summary>
public interface ILanguageRepository : IRepository<Language>
{
    /// <summary>Возвращает список языков, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Language>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает язык по ISO 639-1 коду без трекинга или <c>null</c>.</summary>
    Task<Language?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Возвращает отслеживаемую EF Core сущность для операций записи или <c>null</c>.</summary>
    Task<Language?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новый язык в контекст EF Core.</summary>
    Task AddAsync(Language entity, CancellationToken cancellationToken = default);
}
