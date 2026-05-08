using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после обновления реквизитов.
/// </summary>
internal sealed class OrganizationUpdatedDomainEventHandler(IFusionCache cache)
    : INotificationHandler<OrganizationUpdatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"{nameof(Organization).ToLowerInvariant()}:{notification.OrganizationId}";
        await cache.RemoveAsync(key, token: cancellationToken);
    }
}
