using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Infrastructure.Services;

/// <summary>
/// Сервис определения эффективных прав пользователя в организации.
/// Работает поверх матрицы доступа и ролей участников.
/// </summary>
public interface IOrganizationPermissionService
{
    /// <summary>
    /// Возвращает эффективную базовую роль пользователя в организации.
    /// Учитывает кастомную роль участника (если назначена) или системную базовую роль.
    /// Возвращает <c>null</c>, если пользователь не является участником организации.
    /// </summary>
    /// <param name="profileId">Идентификатор профиля пользователя.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="ct">Токен отмены.</param>
    Task<OrganizationBaseRole?> GetEffectiveRoleAsync(
        Guid profileId,
        Guid organizationId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Проверяет, имеет ли пользователь указанное разрешение в контексте организации.
    /// </summary>
    /// <param name="profileId">Идентификатор профиля пользователя.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="permission">Проверяемое разрешение.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns><c>true</c>, если пользователь имеет разрешение; иначе <c>false</c>.</returns>
    Task<bool> HasPermissionAsync(
        Guid profileId,
        Guid organizationId,
        Permission permission,
        CancellationToken ct = default
    );

    /// <summary>
    /// Проверяет, может ли инициатор управлять целевым пользователем (назначать / снимать роли).
    /// Реализует иерархические ограничения: admin не может управлять owner и т.д.
    /// </summary>
    /// <param name="actorProfileId">Идентификатор инициатора действия.</param>
    /// <param name="targetProfileId">Идентификатор целевого пользователя.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns><c>true</c>, если инициатор вправе управлять целевым пользователем.</returns>
    Task<bool> CanManageUserAsync(
        Guid actorProfileId,
        Guid targetProfileId,
        Guid organizationId,
        CancellationToken ct = default
    );
}
