using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;
using ZiggyCreatures.Caching.Fusion;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после удаления.
/// </summary>
internal sealed class RemoveCache(IFusionCache cache)
    : INotificationHandler<OrganizationDeletedDomainEvent>, INotificationHandler<OrganizationUpdatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"{nameof(Organization).ToLowerInvariant()}:{notification.OrganizationId}";
        await cache.RemoveAsync(key, token: cancellationToken);
    }

    public async ValueTask Handle(
        OrganizationUpdatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var id = notification.OrganizationId;
        var detailKey = $"{nameof(Organization).ToLowerInvariant()}:{id}";
        var summaryKey = $"org:{id}:summary";

        await cache.RemoveAsync(detailKey, token: cancellationToken);
        await cache.RemoveAsync(summaryKey, token: cancellationToken);
    }
}
