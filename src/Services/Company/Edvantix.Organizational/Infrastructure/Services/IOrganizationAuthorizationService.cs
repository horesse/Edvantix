namespace Edvantix.Organizational.Infrastructure.Services;

/// <summary>
/// Сервис авторизации для проверки ролей в организации и группах.
/// </summary>
public interface IOrganizationAuthorizationService
{
    /// <summary>
    /// Возвращает участника организации для текущего пользователя.
    /// Выбрасывает ForbiddenException, если пользователь не является участником.
    /// </summary>
    Task<OrganizationMember> GetCurrentMemberAsync(
        Guid organizationId,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Проверяет, что текущий пользователь имеет одну из указанных ролей в организации.
    /// Выбрасывает ForbiddenException, если роль не соответствует.
    /// </summary>
    Task<OrganizationMember> RequireOrgRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken,
        params OrganizationRole[] allowedRoles
    );

    /// <summary>
    /// Проверяет, что текущий пользователь может управлять группой
    /// (Owner/Manager на уровне организации, или Teacher/Manager на уровне группы).
    /// </summary>
    Task RequireGroupManagementAsync(Guid groupId, CancellationToken cancellationToken);
}
