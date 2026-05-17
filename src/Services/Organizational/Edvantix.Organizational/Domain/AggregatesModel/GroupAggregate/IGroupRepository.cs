using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Проекция одной группы для вычисления KPI-агрегатов.
/// Загружается одним SQL-запросом без материализации сущностей.
/// </summary>
/// <param name="Status">Статус группы.</param>
/// <param name="Capacity">Максимальная вместимость группы.</param>
/// <param name="ActiveMemberCount">Количество активных участников (ExitedAt IS NULL).</param>
public sealed record GroupStatRow(GroupStatus Status, int Capacity, int ActiveMemberCount);

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

    /// <summary>
    /// Возвращает лёгкие проекции групп для вычисления KPI-статистики.
    /// Один SQL-запрос: статус, вместимость и количество активных участников на группу.
    /// </summary>
    Task<IReadOnlyList<GroupStatRow>> GetStatsProjectionAsync(
        Guid organizationId,
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
    /// Возвращает словарь &lt;OrganizationMemberId, OrganizationMember&gt; для указанных участников-преподавателей.
    /// Используется для обогащения DTO данными преподавателя через Persona gRPC.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, OrganizationMember>> GetTeacherMemberInfoAsync(
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
