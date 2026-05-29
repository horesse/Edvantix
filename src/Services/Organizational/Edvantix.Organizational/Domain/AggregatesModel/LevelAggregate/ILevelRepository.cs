namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

/// <summary>Репозиторий агрегата <see cref="Level"/>.</summary>
public interface ILevelRepository : IRepository<Level>
{
    /// <summary>Возвращает уровень по идентификатору.</summary>
    Task<Level?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Проверяет существование уровня по идентификатору.</summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, что уровень существует, принадлежит указанной организации и не удалён.
    /// Если <paramref name="requireActive"/> равно <c>true</c>, дополнительно проверяет активность.
    /// </summary>
    Task<bool> ExistsAsync(
        Guid id,
        Guid organizationId,
        bool requireActive,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает уровни по набору идентификаторов.</summary>
    Task<IReadOnlyCollection<Level>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новый уровень.</summary>
    Task AddAsync(Level level, CancellationToken cancellationToken = default);

    /// <summary>Добавляет массив уровней.</summary>
    Task AddRange(List<Level> levels, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список уровней по спецификации.</summary>
    Task<IReadOnlyList<Level>> ListAsync(
        ISpecification<Level> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает количество уровней по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<Level> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, существует ли не удалённый уровень с указанным кодом в организации,
    /// исключая запись <paramref name="excludeId"/>.
    /// </summary>
    Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, существует ли активный (не удалённый, не деактивированный) уровень
    /// с данным именем в организации, исключая запись <paramref name="excludeId"/>.
    /// </summary>
    Task<bool> ExistsWithNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
}
