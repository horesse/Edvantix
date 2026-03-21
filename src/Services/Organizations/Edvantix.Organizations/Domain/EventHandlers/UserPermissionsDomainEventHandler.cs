using Edvantix.Chassis.EventBus.Dispatcher;
using Edvantix.Organizations.Infrastructure.EventServices.Events;

namespace Edvantix.Organizations.Domain.EventHandlers;

/// <summary>
/// Handles domain events that indicate a user's effective permissions may have changed.
/// Delegates to <see cref="IEventDispatcher"/> which maps each domain event to a
/// <see cref="Edvantix.Contracts.UserPermissionsInvalidatedIntegrationEvent"/> and publishes
/// it via the MassTransit transactional outbox.
/// </summary>
public sealed class UserRoleAssignedEventHandler(
    IEventDispatcher dispatcher
) : INotificationHandler<UserRoleAssignedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(
        UserRoleAssignedEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}

/// <summary>
/// Handles the domain event raised when a role assignment is revoked from a user.
/// Publishes <see cref="Edvantix.Contracts.UserPermissionsInvalidatedIntegrationEvent"/> via outbox.
/// </summary>
public sealed class UserRoleRevokedEventHandler(
    IEventDispatcher dispatcher
) : INotificationHandler<UserRoleRevokedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(
        UserRoleRevokedEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}

/// <summary>
/// Handles the domain event raised when a role's permission set changes.
/// Publishes <see cref="Edvantix.Contracts.UserPermissionsInvalidatedIntegrationEvent"/> with
/// UserId = null to signal school-scoped cache invalidation across all consumers.
/// </summary>
public sealed class RolePermissionsChangedEventHandler(
    IEventDispatcher dispatcher
) : INotificationHandler<RolePermissionsChangedEvent>
{
    /// <inheritdoc/>
    public async ValueTask Handle(
        RolePermissionsChangedEvent notification,
        CancellationToken cancellationToken
    ) => await dispatcher.DispatchAsync(notification, cancellationToken);
}
