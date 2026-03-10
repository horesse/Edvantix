namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Матрица доступа организации: сопоставляет базовые роли с набором permissions
/// и реализует иерархическую логику управления ролями.
/// </summary>
/// <remarks>
/// Абстракция обеспечивает возможность замены хардкодной матрицы (MVP)
/// на конфигурируемую систему без изменения вызывающего кода.
/// </remarks>
public interface IOrganizationPermissionMatrix
{
    /// <summary>
    /// Проверяет, имеет ли роль указанное право доступа.
    /// Наследование: вышестоящие роли включают все права нижестоящих.
    /// </summary>
    /// <param name="role">Базовая роль участника.</param>
    /// <param name="permission">Проверяемое право доступа.</param>
    /// <returns><c>true</c>, если роль обладает данным правом.</returns>
    bool HasPermission(OrganizationBaseRole role, Permission permission);

    /// <summary>
    /// Проверяет, может ли актор управлять (назначать/снимать) целевую роль.
    /// Актор может управлять только ролями строго ниже своей в иерархии.
    /// </summary>
    /// <param name="actorRole">Роль инициатора действия.</param>
    /// <param name="targetRole">Роль, которую пытаются назначить или снять.</param>
    /// <returns><c>true</c>, если актор может управлять целевой ролью.</returns>
    /// <example>
    /// <code>
    /// matrix.CanManageRole(OrganizationBaseRole.Admin, OrganizationBaseRole.Manager); // true
    /// matrix.CanManageRole(OrganizationBaseRole.Admin, OrganizationBaseRole.Owner);   // false
    /// matrix.CanManageRole(OrganizationBaseRole.Owner, OrganizationBaseRole.Owner);   // false
    /// </code>
    /// </example>
    bool CanManageRole(OrganizationBaseRole actorRole, OrganizationBaseRole targetRole);

    /// <summary>
    /// Возвращает полный набор прав доступа для указанной роли (включая унаследованные).
    /// </summary>
    IReadOnlySet<Permission> GetPermissions(OrganizationBaseRole role);
}
