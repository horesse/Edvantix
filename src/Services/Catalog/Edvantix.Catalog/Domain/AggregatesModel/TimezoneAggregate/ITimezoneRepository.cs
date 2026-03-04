namespace Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;

/// <summary>
/// Репозиторий для чтения справочника часовых поясов.
/// </summary>
public interface ITimezoneRepository : IRepository<Timezone>
{
    /// <summary>Возвращает список часовых поясов, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Timezone>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает часовой пояс по IANA-коду без трекинга или <c>null</c>.</summary>
    Task<Timezone?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Возвращает отслеживаемую EF Core сущность для операций записи или <c>null</c>.</summary>
    Task<Timezone?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новый часовой пояс в контекст EF Core.</summary>
    Task AddAsync(Timezone entity, CancellationToken cancellationToken = default);
}
