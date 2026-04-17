using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Инвалидирует запись организации в кэше после удаления.
/// </summary>
internal sealed class OrganizationDeletedDomainEventHandler(IHybridCache cache)
    : INotificationHandler<OrganizationDeletedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationDeletedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var key = $"{nameof(Organization).ToLowerInvariant()}:{notification.OrganizationId}";
        await cache.RemoveAsync(key, cancellationToken);
    }
}
