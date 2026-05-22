namespace Edvantix.Groups.Domain.LessonTypeAggregate;

/// <summary>Репозиторий агрегата <see cref="LessonType"/>.</summary>
public interface ILessonTypeRepository : IRepository<LessonType>
{
    /// <summary>Возвращает тип занятия по идентификатору (без учёта статуса архивирования).</summary>
    Task<LessonType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый тип занятия.</summary>
    Task AddAsync(LessonType lessonType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет уникальность имени в рамках организации среди не архивных записей.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="name">Имя для проверки (уже Trim-нутое).</param>
    /// <param name="excludeId">Идентификатор исключаемой записи (для сценария update).</param>
    Task<bool> NameExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет уникальность кода в рамках организации среди не архивных записей.
    /// </summary>
    Task<bool> CodeExistsAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает страницу типов занятий организации с опциональным поиском.
    /// </summary>
    Task<(IReadOnlyList<LessonType> Items, int TotalCount)> ListAsync(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int offset,
        int limit,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает статистику справочника для указанной организации.
    /// </summary>
    Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
