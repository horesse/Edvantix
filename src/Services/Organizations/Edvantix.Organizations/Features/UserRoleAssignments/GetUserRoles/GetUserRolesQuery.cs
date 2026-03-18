namespace Edvantix.Organizations.Features.UserRoleAssignments.GetUserRoles;

/// <summary>A flattened view of a single role assignment, returned by <see cref="GetUserRolesQuery"/>.</summary>
public sealed record UserRoleItem(Guid AssignmentId, Guid RoleId, string RoleName);

/// <summary>Query to retrieve all roles assigned to a user profile within the current tenant.</summary>
public sealed class GetUserRolesQuery : IQuery<List<UserRoleItem>>
{
    /// <summary>Gets the profile identifier whose roles to return.</summary>
    public required Guid ProfileId { get; init; }
}

/// <summary>
/// Returns all roles assigned to the given profile in the current tenant.
/// Joins assignments with roles in-memory — acceptable for v1 given small role catalogue size.
/// </summary>
public sealed class GetUserRolesQueryHandler(
    IUserRoleAssignmentRepository assignmentRepository,
    IRoleRepository roleRepository
) : IQueryHandler<GetUserRolesQuery, List<UserRoleItem>>
{
    /// <inheritdoc/>
    public async ValueTask<List<UserRoleItem>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var assignments = await assignmentRepository.GetByProfileIdAsync(
            request.ProfileId,
            cancellationToken
        );

        if (assignments.Count == 0)
        {
            return [];
        }

        // Fetch all roles in tenant, then join in-memory.
        // Acceptable for v1 — the tenant role catalogue is small (< 100 rows typical).
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        var roleMap = roles.ToDictionary(r => r.Id, r => r.Name);

        return assignments
            .Where(a => roleMap.ContainsKey(a.RoleId))
            .Select(a => new UserRoleItem(a.Id, a.RoleId, roleMap[a.RoleId]))
            .OrderBy(r => r.RoleName)
            .ToList();
    }
}
