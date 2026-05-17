using System.Reflection;
using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Grpc.Services;
using IMessage = Mediator.IMessage;

namespace Edvantix.Groups.Pipelines;

/// <summary>
/// Пре-процессор Mediator: проверяет, что профиль является активным участником организации
/// и имеет разрешение, указанное в <see cref="RequirePermissionAttribute"/>.
/// Проверка выполняется через gRPC-вызов к Organizational (с локальным кешем в <see cref="IPermissionService"/>).
/// </summary>
internal sealed class AuthorizationBehavior<TMessage, TResponse>(
    ClaimsPrincipal claims,
    ITenantContext tenantContext,
    IPermissionService permissionService,
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

        if (!tenantContext.IsResolved)
        {
            logger.LogWarning(
                "[AuthorizationBehavior] X-Organization-Id header missing for {Message}",
                message.GetType().Name
            );
            throw new ForbiddenException("Контекст организации не определён.");
        }

        var organizationId = tenantContext.OrganizationId;
        var profileId = claims.GetProfileIdOrError();

        var hasPermission = await permissionService.CheckPermissionAsync(
            organizationId,
            profileId,
            attr.Permission,
            cancellationToken
        );

        if (!hasPermission)
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
