using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

/// <summary>Репозиторий агрегата <see cref="StudentStatus"/>.</summary>
public interface IStudentStatusRepository : IRepository<StudentStatus>
{
    /// <summary>Добавляет новый статус.</summary>
    Task AddAsync(StudentStatus status, CancellationToken ct = default);

    /// <summary>Добавляет несколько статусов (используется при сидинге).</summary>
    Task AddRangeAsync(IEnumerable<StudentStatus> statuses, CancellationToken ct = default);

    /// <summary>Возвращает статус по идентификатору (включая архивные).</summary>
    Task<StudentStatus?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Возвращает список статусов, удовлетворяющих спецификации.</summary>
    Task<IReadOnlyList<StudentStatus>> ListAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    );

    /// <summary>Возвращает количество статусов, удовлетворяющих спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    );

    /// <summary>
    /// Возвращает <see langword="true"/>, если существует хотя бы один статус,
    /// удовлетворяющий спецификации.
    /// </summary>
    Task<bool> AnyAsync(
        ISpecification<StudentStatus> specification,
        CancellationToken ct = default
    );

    /// <summary>Дата последнего изменения любой записи организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
