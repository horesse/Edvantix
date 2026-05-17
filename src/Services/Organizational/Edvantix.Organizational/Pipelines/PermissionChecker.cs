using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Pipelines;

/// <summary>
/// Реализует двухуровневую проверку разрешений с FusionCache:
/// <list type="bullet">
///   <item><description>L1 — связка «участник → roleId» (уникальна для каждого участника).</description></item>
///   <item><description>L2 — разрешения роли по roleId (разделяется между участниками с одной ролью).</description></item>
/// </list>
/// Теги инвалидации соответствуют <see cref="AuthorizationCacheKeys"/>.
/// </summary>
internal sealed class PermissionChecker(
    IOrganizationMemberRepository memberRepository,
    IOrganizationRoleRepository roleRepository,
    IFusionCache cache,
    ILogger<PermissionChecker> logger
) : IPermissionChecker
{
    public async Task<bool?> CheckAsync(
        Guid organizationId,
        Guid profileId,
        string permission,
        CancellationToken cancellationToken
    )
    {
        // L1: получаем roleId участника. Guid.Empty — сигнал об отсутствии активного членства.
        var roleId = await cache.GetOrSetAsync(
            AuthorizationCacheKeys.MemberRole(organizationId, profileId),
            async ct =>
                await memberRepository.GetActiveMemberRoleIdAsync(organizationId, profileId, ct)
                ?? Guid.Empty,
            tags: [AuthorizationCacheKeys.OrgPermsTag(organizationId)],
            token: cancellationToken
        );

        if (roleId == Guid.Empty)
        {
            logger.LogWarning(
                "[PermissionChecker] Profile {ProfileId} is not an active member of org {OrgId}",
                profileId,
                organizationId
            );
            return null;
        }

        // L2: разрешения роли — общий кеш для всех участников с одинаковой ролью.
        var permissions = await cache.GetOrSetAsync<HashSet<string>>(
            AuthorizationCacheKeys.RolePerms(roleId),
            async ct =>
            {
                var role = await roleRepository.GetByIdWithPermissionsAsync(roleId, ct);
                return role is null
                    ? []
                    : role
                        .Permissions.Select(p => p.FullCode)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
            },
            tags:
            [
                AuthorizationCacheKeys.RolePerms(roleId),
                AuthorizationCacheKeys.OrgPermsTag(organizationId),
            ],
            token: cancellationToken
        );

        return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }
}
