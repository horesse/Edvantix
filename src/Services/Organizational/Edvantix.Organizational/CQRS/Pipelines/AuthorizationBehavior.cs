using System.Reflection;
using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.CQRS.Pipelines;

/// <summary>
/// Пре-процессор Mediator: проверяет, что профиль является активным участником организации
/// и имеет разрешение, указанное в <see cref="RequirePermissionAttribute"/> на команде или запросе.
/// Разрешения кешируются в HybridCache для снижения нагрузки на БД.
/// </summary>
internal sealed class AuthorizationBehavior<TMessage, TResponse>(
    ClaimsPrincipal claims,
    ITenantContext tenantContext,
    IOrganizationMemberRepository memberRepository,
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

        var cacheKey = $"perm:org:{organizationId}:profile:{profileId}";
        string[] tags = [$"org-perms:{organizationId}", $"profile-perms:{profileId}"];

        var permissions = await cache.GetOrCreateAsync<HashSet<string>>(
            cacheKey,
            async ct =>
                await memberRepository.GetActivePermissionsAsync(organizationId, profileId, ct),
            tags,
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
