namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>Репозиторий агрегата <see cref="Course"/>.</summary>
public interface ICourseRepository : IRepository<Course>
{
    /// <summary>Возвращает курс по идентификатору (без children, только для чтения).</summary>
    Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает курс с модулями, уроками и целями (отслеживание EF, для записи).</summary>
    Task<Course?> GetByIdForWriteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает курс с полной загрузкой children (AsNoTracking, для детальной страницы).</summary>
    Task<Course?> GetByIdWithModulesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает курс, содержащий модуль с указанным идентификатором (для записи).</summary>
    Task<Course?> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>Возвращает курс, содержащий урок с указанным идентификатором (для записи).</summary>
    Task<Course?> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый курс в контекст.</summary>
    Task AddAsync(Course course, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список курсов по спецификации.</summary>
    Task<IReadOnlyList<Course>> ListAsync(
        Specification<Course> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает количество курсов по спецификации.</summary>
    Task<int> CountAsync(
        Specification<Course> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает список не удалённых курсов по набору идентификаторов (AsNoTracking).
    /// Используется gRPC-эндпойнтом GetCoursesByIds для массового обогащения данных.
    /// </summary>
    Task<IReadOnlyList<Course>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default
    );
}
