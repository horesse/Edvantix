using Edvantix.Chassis.Caching;

namespace Edvantix.Organizations.Features.Permissions.CheckPermission;

/// <summary>Query that checks whether a user holds a specific permission within a school.</summary>
/// <param name="UserId">The user profile identifier.</param>
/// <param name="SchoolId">The school (tenant) identifier.</param>
/// <param name="Permission">The permission string to check (e.g., "scheduling:create-slot").</param>
public sealed record CheckPermissionQuery(Guid UserId, Guid SchoolId, string Permission)
    : IQuery<bool>;

/// <summary>
/// Resolves the user's effective permissions for a school using a two-level cache
/// (L1 in-memory, L2 Redis) via <see cref="IHybridCache"/>.
///
/// Cache key: <c>perm:{userId}:{schoolId}:{permission}</c>
/// Cache tag:  <c>user:{userId}:{schoolId}</c> — used for bulk invalidation when
///             a role assignment changes.
///
/// IMPORTANT: This handler bypasses the EF Core tenant query filters because gRPC calls
/// do not pass through TenantMiddleware and therefore have no ambient tenant context.
/// Repository methods that ignore query filters are used, with explicit schoolId filtering
/// to preserve data isolation guarantees.
/// </summary>
public sealed class CheckPermissionQueryHandler(
    IHybridCache cache,
    IUserRoleAssignmentRepository assignmentRepository,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository
) : IQueryHandler<CheckPermissionQuery, bool>
{
    /// <inheritdoc/>
    public ValueTask<bool> Handle(
        CheckPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        var key = $"perm:{request.UserId}:{request.SchoolId}:{request.Permission}";
        var tag = $"user:{request.UserId}:{request.SchoolId}";

        return cache.GetOrCreateAsync(
            key,
            ct => ResolvePermissionAsync(request, ct),
            tags: [tag],
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// Loads assignments → roles → permissions from DB and checks whether the requested
    /// permission is present in any of the user's roles for the given school.
    /// </summary>
    private async ValueTask<bool> ResolvePermissionAsync(
        CheckPermissionQuery request,
        CancellationToken cancellationToken
    )
    {
        // 1. Load all role assignments for this user+school, bypassing the tenant filter.
        var assignments = await assignmentRepository.GetByProfileAndSchoolAsync(
            request.UserId,
            request.SchoolId,
            cancellationToken
        );

        if (assignments.Count == 0)
        {
            return false;
        }

        var roleIds = assignments.Select(a => a.RoleId).ToHashSet();

        // 2. Load all roles for the school with their permission IDs eagerly loaded,
        //    bypassing the tenant query filter (no ambient tenant in gRPC context).
        var roles = await roleRepository.GetBySchoolAsync(request.SchoolId, cancellationToken);
        var assignedRoles = roles.Where(r => roleIds.Contains(r.Id)).ToList();

        if (assignedRoles.Count == 0)
        {
            return false;
        }

        // 3. Collect all permission IDs from the user's roles.
        var permissionIds = assignedRoles
            .SelectMany(r => r.Permissions.Select(rp => rp.PermissionId))
            .ToHashSet();

        if (permissionIds.Count == 0)
        {
            return false;
        }

        // 4. Resolve permission names from the global catalogue and check membership.
        //    Permission catalogue is small (tens of entries) — loading all and filtering
        //    in-memory is acceptable for v1.
        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        return allPermissions
            .Where(p => permissionIds.Contains(p.Id))
            .Any(p =>
                string.Equals(p.Name, request.Permission, StringComparison.OrdinalIgnoreCase)
            );
    }
}
