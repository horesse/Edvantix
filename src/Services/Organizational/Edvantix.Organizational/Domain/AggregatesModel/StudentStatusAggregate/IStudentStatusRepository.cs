using Edvantix.Chassis.Repository;
using Edvantix.SharedKernel.Results;

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

    /// <summary>
    /// Постраничный список статусов организации.
    /// </summary>
    Task<PagedResult<StudentStatus>> ListAsync(
        Guid organizationId,
        bool includeArchived,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct = default
    );

    /// <summary>Проверяет уникальность имени в рамках не архивных записей организации.</summary>
    Task<bool> ExistsNameAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct = default
    );

    /// <summary>Проверяет уникальность кода в рамках не архивных записей организации.</summary>
    Task<bool> ExistsCodeAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken ct = default
    );

    /// <summary>Количество активных (не архивных) записей организации.</summary>
    Task<int> CountActiveAsync(Guid organizationId, CancellationToken ct = default);

    /// <summary>Количество архивных записей организации.</summary>
    Task<int> CountArchivedAsync(Guid organizationId, CancellationToken ct = default);

    /// <summary>Дата последнего изменения любой записи организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
