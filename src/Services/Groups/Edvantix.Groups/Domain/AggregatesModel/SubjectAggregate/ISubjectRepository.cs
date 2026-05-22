namespace Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;

/// <summary>Репозиторий агрегата <see cref="Subject"/>.</summary>
public interface ISubjectRepository : IRepository<Subject>
{
    /// <summary>Возвращает предмет по идентификатору.</summary>
    Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый предмет.</summary>
    Task AddAsync(Subject subject, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает постраничный список предметов организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="search">Текстовый поиск по названию (опционально).</param>
    /// <param name="includeArchived">Включить архивные записи.</param>
    /// <param name="offset">Смещение (пропустить первые N записей).</param>
    /// <param name="size">Размер страницы.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Subject>> ListAsync(
        Guid organizationId,
        string? search,
        bool includeArchived,
        int offset,
        int size,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает общее число предметов с учётом фильтров.</summary>
    Task<long> CountAsync(
        Guid organizationId,
        string? search,
        bool includeArchived,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, существует ли не архивный предмет с указанным кодом в организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="code">Нормализованный код предмета.</param>
    /// <param name="excludeId">Идентификатор предмета, исключаемого из проверки (для update-сценария).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, существует ли не архивный предмет с указанным именем в организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="name">Имя предмета (после Trim).</param>
    /// <param name="excludeId">Идентификатор предмета, исключаемого из проверки (для update-сценария).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<bool> ExistsWithNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает статистику предметов организации для справочника настроек.</summary>
    Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
