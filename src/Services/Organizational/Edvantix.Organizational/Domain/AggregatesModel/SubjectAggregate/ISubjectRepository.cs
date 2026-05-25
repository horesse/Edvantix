namespace Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;

/// <summary>Репозиторий агрегата <see cref="Subject"/>.</summary>
public interface ISubjectRepository : IRepository<Subject>
{
    /// <summary>Возвращает предмет по идентификатору.</summary>
    Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый предмет.</summary>
    Task AddAsync(Subject subject, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список предметов, соответствующих спецификации.</summary>
    Task<IReadOnlyList<Subject>> ListAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает количество предметов, соответствующих спецификации.</summary>
    Task<long> CountAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает <c>true</c>, если хотя бы один предмет соответствует спецификации.
    /// Используется для проверок уникальности по названию.
    /// </summary>
    Task<bool> AnyAsync(
        ISpecification<Subject> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Проверяет, существует ли не архивный предмет с указанным кодом в организации.
    /// <para>
    /// Сравнение выполняется в памяти: <see cref="SubjectCode"/> хранится через value-конвертер,
    /// и его прямое сравнение в SQL-выражениях не поддерживается.
    /// </para>
    /// </summary>
    Task<bool> ExistsWithCodeAsync(
        Guid organizationId,
        SubjectCode code,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает статистику предметов организации для справочника настроек.</summary>
    Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
