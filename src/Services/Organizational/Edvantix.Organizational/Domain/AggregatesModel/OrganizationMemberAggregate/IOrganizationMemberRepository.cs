namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>Репозиторий агрегата <see cref="OrganizationMember"/>.</summary>
public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    /// <summary>Возвращает участника по идентификатору.</summary>
    Task<OrganizationMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает всех участников по спецификации.</summary>
    Task<IReadOnlyCollection<OrganizationMember>> ListAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает количество участников по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет участника.</summary>
    Task AddAsync(OrganizationMember member, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, что участник с указанным идентификатором принадлежит организации и не удалён.
    /// </summary>
    Task<bool> ExistsAsync(
        Guid id,
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает идентификатор роли активного участника организации.
    /// Возвращает <see langword="null"/>, если участник не найден или не активен.
    /// </summary>
    Task<Guid?> GetActiveMemberRoleIdAsync(
        Guid organizationId,
        Guid profileId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает количество активных участников с заданной ролью.</summary>
    Task<int> CountByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает словарь {roleId → count} активных участников для набора ролей.
    /// Отсутствующие идентификаторы означают 0 участников.
    /// </summary>
    Task<IReadOnlyDictionary<Guid, int>> GetMemberCountsByRolesAsync(
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken = default
    );
}
