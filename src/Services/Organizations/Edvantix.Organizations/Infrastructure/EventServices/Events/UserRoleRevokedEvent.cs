using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizations.Infrastructure.EventServices.Events;

/// <summary>
/// Domain event raised when a role assignment is revoked from a user within a school.
/// Triggers cache invalidation and outbox publication of <see cref="Edvantix.Contracts.UserPermissionsInvalidatedIntegrationEvent"/>.
/// </summary>
public sealed class UserRoleRevokedEvent(Guid profileId, Guid schoolId, Guid roleId) : DomainEvent
{
    /// <summary>Gets the profile identifier of the user whose role was revoked.</summary>
    public Guid ProfileId { get; } = profileId;

    /// <summary>Gets the school in which the revocation occurred.</summary>
    public Guid SchoolId { get; } = schoolId;

    /// <summary>Gets the role that was revoked.</summary>
    public Guid RoleId { get; } = roleId;
}
