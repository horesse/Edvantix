using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizations.Infrastructure.EventServices.Events;

/// <summary>
/// Domain event raised when a role is assigned to a user within a school.
/// Triggers cache invalidation and outbox publication of <see cref="Edvantix.Contracts.UserPermissionsInvalidatedIntegrationEvent"/>.
/// </summary>
public sealed class UserRoleAssignedEvent(Guid profileId, Guid schoolId, Guid roleId) : DomainEvent
{
    /// <summary>Gets the profile identifier of the user who received the role.</summary>
    public Guid ProfileId { get; } = profileId;

    /// <summary>Gets the school in which the assignment was made.</summary>
    public Guid SchoolId { get; } = schoolId;

    /// <summary>Gets the role that was assigned.</summary>
    public Guid RoleId { get; } = roleId;
}
