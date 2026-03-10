namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Репозиторий для работы с кастомными ролями организации.
/// </summary>
public interface IOrganizationCustomRoleRepository : IRepository<OrganizationCustomRole>
{
    /// <summary>
    /// Находит кастомную роль по ID в рамках организации (только не удалённые).
    /// </summary>
    Task<OrganizationCustomRole?> FindByIdAsync(
        Guid id,
        Guid organizationId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Находит кастомную роль по коду в рамках организации (только среди не удалённых).
    /// </summary>
    Task<OrganizationCustomRole?> FindByCodeAsync(
        Guid organizationId,
        string code,
        CancellationToken ct = default
    );

    /// <summary>
    /// Возвращает все кастомные роли организации (только не удалённые).
    /// </summary>
    Task<IReadOnlyList<OrganizationCustomRole>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Создаёт новую кастомную роль в хранилище.
    /// </summary>
    Task<OrganizationCustomRole> AddAsync(
        OrganizationCustomRole role,
        CancellationToken ct = default
    );

    /// <summary>
    /// Возвращает количество активных участников, которым назначена данная кастомная роль.
    /// Используется для проверки перед изменением кода или удалением роли.
    /// </summary>
    Task<int> GetAssignedMembersCountAsync(Guid roleId, CancellationToken ct = default);
}
