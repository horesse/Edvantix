namespace Edvantix.Organizational.Domain.LessonTypeAggregate;

/// <summary>Репозиторий агрегата <see cref="LessonType"/>.</summary>
public interface ILessonTypeRepository : IRepository<LessonType>
{
    /// <summary>Возвращает тип занятия по идентификатору (без учёта статуса архивирования).</summary>
    Task<LessonType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый тип занятия.</summary>
    Task AddAsync(LessonType lessonType, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список типов занятий, соответствующих спецификации.</summary>
    Task<IReadOnlyList<LessonType>> ListAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает количество записей, соответствующих спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает <c>true</c>, если существует хотя бы одна запись по спецификации.</summary>
    Task<bool> AnyAsync(
        ISpecification<LessonType> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает статистику справочника для указанной организации.</summary>
    Task<(int ActiveCount, int ArchivedCount, DateTime? LastModifiedAt)> GetStatsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
