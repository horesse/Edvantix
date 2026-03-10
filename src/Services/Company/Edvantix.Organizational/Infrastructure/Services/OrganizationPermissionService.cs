using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Infrastructure.Services;

/// <summary>
/// Реализация сервиса определения эффективных прав пользователя в организации.
/// Объединяет матрицу доступа, системные роли участников и (в будущем) кастомные роли.
/// </summary>
public sealed class OrganizationPermissionService(
    IOrganizationMemberRepository memberRepo,
    IOrganizationPermissionMatrix permissionMatrix
) : IOrganizationPermissionService
{
    /// <inheritdoc/>
    public async Task<OrganizationBaseRole?> GetEffectiveRoleAsync(
        Guid profileId,
        Guid organizationId,
        CancellationToken ct = default
    )
    {
        var spec = new OrganizationMemberSpecification(profileId, organizationId);
        var member = await memberRepo.FindAsync(spec, ct);

        if (member is null)
        {
            return null;
        }

        // TODO: Когда будет реализовано назначение кастомных ролей участникам,
        // здесь нужно проверить наличие кастомной роли и вернуть её BaseRole.
        return ToBaseRole(member.Role);
    }

    /// <inheritdoc/>
    public async Task<bool> HasPermissionAsync(
        Guid profileId,
        Guid organizationId,
        Permission permission,
        CancellationToken ct = default
    )
    {
        var effectiveRole = await GetEffectiveRoleAsync(profileId, organizationId, ct);

        if (effectiveRole is null)
        {
            // Пользователь не является участником организации.
            return false;
        }

        return permissionMatrix.HasPermission(effectiveRole.Value, permission);
    }

    /// <inheritdoc/>
    public async Task<bool> CanManageUserAsync(
        Guid actorProfileId,
        Guid targetProfileId,
        Guid organizationId,
        CancellationToken ct = default
    )
    {
        var actorRole = await GetEffectiveRoleAsync(actorProfileId, organizationId, ct);
        var targetRole = await GetEffectiveRoleAsync(targetProfileId, organizationId, ct);

        if (actorRole is null || targetRole is null)
        {
            return false;
        }

        // Инициатор должен иметь право назначать роли (RoleAssign — admin и выше)
        // И его роль должна быть выше роли целевого пользователя в иерархии.
        return permissionMatrix.HasPermission(actorRole.Value, Permission.RoleAssign)
            && permissionMatrix.CanManageRole(actorRole.Value, targetRole.Value);
    }

    /// <summary>
    /// Конвертирует системную роль участника организации в базовую роль матрицы доступа.
    /// <para>
    /// <see cref="OrganizationRole"/> не включает <see cref="OrganizationBaseRole.Admin"/> —
    /// эта роль доступна только через кастомные роли.
    /// </para>
    /// </summary>
    private static OrganizationBaseRole ToBaseRole(OrganizationRole role) =>
        role switch
        {
            OrganizationRole.Owner => OrganizationBaseRole.Owner,
            OrganizationRole.Manager => OrganizationBaseRole.Manager,
            OrganizationRole.Teacher => OrganizationBaseRole.Teacher,
            OrganizationRole.Student => OrganizationBaseRole.Student,
            _ => throw new ArgumentOutOfRangeException(
                nameof(role),
                role,
                $"Неизвестная системная роль организации: {role}."
            ),
        };
}
