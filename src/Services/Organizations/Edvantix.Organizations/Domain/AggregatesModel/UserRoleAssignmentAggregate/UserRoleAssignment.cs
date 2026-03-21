using Edvantix.Organizations.Infrastructure.EventServices.Events;

namespace Edvantix.Organizations.Domain.AggregatesModel.UserRoleAssignmentAggregate;

/// <summary>
/// Records the assignment of a <c>Role</c> to a user profile within a specific school.
/// Tenant-scoped — filtered by <c>SchoolId</c> in <c>OrganizationsDbContext</c>.
/// Uniqueness is enforced at the database level: one profile cannot hold the same role
/// twice within the same school.
/// </summary>
public sealed class UserRoleAssignment : Entity, IAggregateRoot, ITenanted
{
    /// <summary>Gets the profile (user) identifier.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Gets the school this assignment belongs to.</summary>
    public Guid SchoolId { get; private set; }

    /// <summary>Gets the role being assigned.</summary>
    public Guid RoleId { get; private set; }

    // EF Core constructor
    private UserRoleAssignment() { }

    /// <summary>Initializes a new role assignment and raises <see cref="UserRoleAssignedEvent"/>.</summary>
    /// <param name="profileId">The user profile identifier. Must not be empty.</param>
    /// <param name="schoolId">The school identifier. Must not be empty.</param>
    /// <param name="roleId">The role identifier. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when any of the identifiers are empty.</exception>
    public UserRoleAssignment(Guid profileId, Guid schoolId, Guid roleId)
    {
        Guard.Against.Default(profileId, nameof(profileId));
        Guard.Against.Default(schoolId, nameof(schoolId));
        Guard.Against.Default(roleId, nameof(roleId));

        ProfileId = profileId;
        SchoolId = schoolId;
        RoleId = roleId;

        // Notify interested parties (cache invalidation, outbox) that a role was assigned.
        RegisterDomainEvent(new UserRoleAssignedEvent(profileId, schoolId, roleId));
    }

    /// <summary>
    /// Marks this assignment as revoked by raising <see cref="UserRoleRevokedEvent"/>.
    /// Must be called before <c>Remove</c> so the event dispatcher can pick it up during SaveEntitiesAsync.
    /// </summary>
    public void Revoke()
    {
        RegisterDomainEvent(new UserRoleRevokedEvent(ProfileId, SchoolId, RoleId));
    }
}
