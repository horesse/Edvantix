using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Репозиторий агрегата <see cref="Group"/>.</summary>
public interface IGroupRepository : IRepository<Group>
{
    /// <summary>Возвращает группу по идентификатору, включая участников.</summary>
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все группы организации.</summary>
    Task<IReadOnlyCollection<Group>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает группы по спецификации.</summary>
    Task<IReadOnlyCollection<Group>> ListAsync(
        ISpecification<Group> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает группы по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<Group> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новую группу.</summary>
    Task AddAsync(Group group, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает коды всех активных групп организации.
    /// Используется для генерации уникального кода новой группы.
    /// </summary>
    Task<IReadOnlyCollection<string>> GetCodesByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает словарь &lt;OrganizationMemberId, ProfileId&gt; для указанных участников-преподавателей.
    /// Используется для обогащения DTO именем преподавателя через Persona gRPC.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, Guid>> GetTeacherProfileIdsAsync(
        IEnumerable<Guid> teacherMemberIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает кабинеты по списку идентификаторов.
    /// Используется для обогащения DTO меткой кабинета.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, Room>> GetRoomsByIdsAsync(
        IEnumerable<Guid> roomIds,
        CancellationToken cancellationToken = default
    );
}
