namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

/// <summary>Репозиторий агрегата <see cref="Level"/>.</summary>
public interface ILevelRepository : IRepository<Level>
{
    /// <summary>Возвращает уровень по идентификатору.</summary>
    Task<Level?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Проверяет существование уровня по идентификатору.</summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает уровни по набору идентификаторов.</summary>
    Task<IReadOnlyCollection<Level>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, используется ли уровень хотя бы одной группой.
    /// Используется в команде удаления для защиты от каскадного удаления.
    /// </summary>
    Task<bool> IsUsedByGroupsAsync(Guid levelId, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый уровень.</summary>
    Task AddAsync(Level level, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все уровни организации, отсортированные по <see cref="Level.SortOrder"/>.</summary>
    Task<IReadOnlyCollection<Level>> ListByOrganizationAsync(
        Guid organizationId,
        bool includeInactive = false,
        CancellationToken cancellationToken = default
    );

    /// <summary>Проверяет, существует ли не удалённый уровень с указанным кодом в организации.</summary>
    Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        string code,
        CancellationToken cancellationToken = default
    );
}
