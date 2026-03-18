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

    /// <summary>Initializes a new role assignment.</summary>
    public UserRoleAssignment(Guid profileId, Guid schoolId, Guid roleId)
    {
        ProfileId = profileId;
        SchoolId = schoolId;
        RoleId = roleId;
    }
}
