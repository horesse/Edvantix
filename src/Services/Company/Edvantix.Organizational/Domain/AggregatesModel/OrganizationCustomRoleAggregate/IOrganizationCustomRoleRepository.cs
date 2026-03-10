namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Репозиторий для работы с кастомными ролями организации.
/// </summary>
public interface IOrganizationCustomRoleRepository : IRepository<OrganizationCustomRole>
{
    /// <summary>
    /// Находит кастомную роль по коду в рамках организации (только среди не удалённых).
    /// </summary>
    Task<OrganizationCustomRole?> FindByCodeAsync(
        Guid organizationId,
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Возвращает все кастомные роли организации (только не удалённые).
    /// </summary>
    Task<IReadOnlyList<OrganizationCustomRole>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );
}
