using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

/// <summary>Репозиторий агрегата <see cref="Room"/>.</summary>
public interface IRoomRepository : IRepository<Room>
{
    /// <summary>Добавляет новый кабинет.</summary>
    Task AddAsync(Room room, CancellationToken ct = default);

    /// <summary>Возвращает кабинет по идентификатору (включая архивные).</summary>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Возвращает список кабинетов, удовлетворяющих спецификации.</summary>
    Task<IReadOnlyList<Room>> ListAsync(
        ISpecification<Room> specification,
        CancellationToken ct = default
    );

    /// <summary>Возвращает количество кабинетов, удовлетворяющих спецификации.</summary>
    Task<int> CountAsync(ISpecification<Room> specification, CancellationToken ct = default);

    /// <summary>
    /// Возвращает <see langword="true"/>, если существует хотя бы один кабинет,
    /// удовлетворяющий спецификации.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<Room> specification, CancellationToken ct = default);

    /// <summary>Дата последнего изменения любой записи организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
