namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

/// <summary>Репозиторий агрегата <see cref="Room"/>.</summary>
public interface IRoomRepository : IRepository<Room>
{
    /// <summary>Возвращает кабинет по идентификатору.</summary>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все активные кабинеты организации.</summary>
    Task<IReadOnlyCollection<Room>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новый кабинет.</summary>
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
}
