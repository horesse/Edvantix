using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Contracts;
using Edvantix.Organizations.Infrastructure.EventServices.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizations.Infrastructure.EventServices;

/// <summary>
/// Maps Organizations domain events to integration events for the MassTransit outbox.
/// Each mapping produces a <see cref="UserPermissionsInvalidatedIntegrationEvent"/> that
/// downstream services (e.g. permission caches) subscribe to for invalidation.
/// </summary>
internal sealed class EventMapper : IEventMapper
{
    /// <inheritdoc/>
    public IntegrationEvent MapToIntegrationEvent(DomainEvent @event)
    {
        return @event switch
        {
            // Role assigned — invalidate cache for the specific user in the school.
            UserRoleAssignedEvent e => new UserPermissionsInvalidatedIntegrationEvent(
                e.ProfileId,
                e.SchoolId,
                DateTimeOffset.UtcNow
            ),

            // Role revoked — invalidate cache for the specific user in the school.
            UserRoleRevokedEvent e => new UserPermissionsInvalidatedIntegrationEvent(
                e.ProfileId,
                e.SchoolId,
                DateTimeOffset.UtcNow
            ),

            // Role permissions changed — affects all users with this role in the school.
            // UserId = null signals school-scoped invalidation to consumers.
            RolePermissionsChangedEvent e => new UserPermissionsInvalidatedIntegrationEvent(
                null,
                e.SchoolId,
                DateTimeOffset.UtcNow
            ),

            _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null),
        };
    }
}
