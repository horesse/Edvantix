using System.Reflection;
using Edvantix.Chassis.CQRS;
using IMessage = Mediator.IMessage;

namespace Edvantix.Organizational.Pipelines;

/// <summary>
/// Пре-процессор Mediator: проверяет, что профиль является активным участником организации
/// и имеет разрешение, указанное в <see cref="RequirePermissionAttribute"/> на команде или запросе.
/// Логика резолва прав (L1/L2-кеш) делегирована <see cref="IPermissionChecker"/>.
/// </summary>
internal sealed class AuthorizationBehavior<TMessage, TResponse>(
    ClaimsPrincipal claims,
    ITenantContext tenantContext,
    IPermissionChecker permissionChecker,
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

        var result = await permissionChecker.CheckAsync(
            organizationId,
            profileId,
            attr.Permission,
            cancellationToken
        );

        if (result is null)
        {
            logger.LogWarning(
                "[AuthorizationBehavior] Profile {ProfileId} is not an active member of org {OrgId}",
                profileId,
                organizationId
            );
            throw new ForbiddenException("Профиль не является активным участником организации.");
        }

        if (!result.Value)
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
