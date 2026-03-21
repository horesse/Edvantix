using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizations.Infrastructure.EventServices.Events;

/// <summary>
/// Domain event raised when a role's permission set is replaced via <c>SetPermissions</c>.
/// Because many users may share the role, the integration event uses a school-scoped tag
/// (UserId = null) to invalidate cache entries for all affected users at once.
/// </summary>
public sealed class RolePermissionsChangedEvent(Guid roleId, Guid schoolId) : DomainEvent
{
    /// <summary>Gets the role whose permissions were changed.</summary>
    public Guid RoleId { get; } = roleId;

    /// <summary>Gets the school that owns the role.</summary>
    public Guid SchoolId { get; } = schoolId;
}
