using System.Reflection;
using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Pipelines;

/// <summary>
/// Пре-процессор Mediator: проверяет, что профиль является активным участником организации
/// и имеет разрешение, указанное в <see cref="RequirePermissionAttribute"/> на команде или запросе.
/// Используется двухуровневый кеш:
/// <list type="bullet">
///   <item><description>L1 — связка «участник → roleId» (уникальна для каждого участника).</description></item>
///   <item><description>L2 — разрешения роли по roleId (разделяется между всеми участниками с одной ролью).</description></item>
/// </list>
/// </summary>
internal sealed class AuthorizationBehavior<TMessage, TResponse>(
    ClaimsPrincipal claims,
    ITenantContext tenantContext,
    IOrganizationMemberRepository memberRepository,
    IOrganizationMemberRoleRepository roleRepository,
    IHybridCache cache,
    ILogger<AuthorizationBehavior<TMessage, TResponse>> logger
) : MessagePreProcessor<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : notnull
{
    protected override async ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        var attr = message.GetType().GetCustomAttribute<RequirePermissionAttribute>();
        if (attr is null)
        {
            return;
        }

        var profileId = claims.GetProfileIdOrError();

        if (!tenantContext.IsResolved)
        {
            logger.LogWarning(
                "[AuthorizationBehavior] X-Organization-Id header missing for {Message}",
                message.GetType().Name
            );
            throw new ForbiddenException("Контекст организации не определён.");
        }

        var organizationId = tenantContext.OrganizationId;

        // Уровень 1: получаем roleId участника. Guid.Empty — сигнал об отсутствии активного членства.
        var memberRoleKey = $"member-role:org:{organizationId}:profile:{profileId}";
        var roleId = await cache.GetOrCreateAsync(
            memberRoleKey,
            async ct =>
                await memberRepository.GetActiveMemberRoleIdAsync(organizationId, profileId, ct)
                ?? Guid.Empty,
            [$"org-perms:{organizationId}"],
            cancellationToken
        );

        if (roleId == Guid.Empty)
        {
            logger.LogWarning(
                "[AuthorizationBehavior] Profile {ProfileId} is not an active member of org {OrgId}",
                profileId,
                organizationId
            );
            throw new ForbiddenException("Профиль не является активным участником организации.");
        }

        // Уровень 2: разрешения роли — общий кеш для всех участников с одинаковой ролью.
        var rolePermsKey = $"role-perms:{roleId}";
        var permissions = await cache.GetOrCreateAsync<HashSet<string>>(
            rolePermsKey,
            async ct =>
            {
                var role = await roleRepository.GetByIdWithPermissionsAsync(roleId, ct);
                return role is null
                    ? []
                    : role
                        .Permissions.Select(p => p.Name)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
            },
            [$"role-perms:{roleId}", $"org-perms:{organizationId}"],
            cancellationToken
        );

        if (!permissions.Contains(attr.Permission, StringComparer.OrdinalIgnoreCase))
        {
            logger.LogWarning(
                "[AuthorizationBehavior] Profile {ProfileId} denied permission {Permission} in org {OrgId}",
                profileId,
                attr.Permission,
                organizationId
            );
            throw new ForbiddenException(
                $"Нет разрешения '{attr.Permission}' в данной организации."
            );
        }
    }
}
