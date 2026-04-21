using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует кеш связки «участник → роль» после деактивации или удаления участника.
/// При следующем обращении запрос вернёт null, что вызовет ForbiddenException.
/// </summary>
internal sealed class OrganizationMemberStatusChangedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationMemberStatusChangedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationMemberStatusChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"member-role:org:{notification.OrganizationId}:profile:{notification.ProfileId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
