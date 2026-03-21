using Edvantix.Chassis.Caching;

namespace Edvantix.Organizations.Features.Roles.AssignPermissions;

/// <summary>
/// Command to assign a set of permissions to a role, replacing any existing assignment.
/// All permission names must exist in the global catalogue.
/// </summary>
public sealed class AssignPermissionsCommand : ICommand<Unit>
{
    public required Guid RoleId { get; init; }

    /// <summary>
    /// Permission names to assign. An empty list clears all permissions from the role.
    /// </summary>
    public required List<string> PermissionNames { get; init; }
}

/// <summary>
/// Validates all provided permission names against the global catalogue before assigning.
/// Calls <see cref="Role.SetPermissions"/> which raises a <c>RolePermissionsChangedEvent</c>
/// domain event — dispatched via <c>SaveEntitiesAsync</c> for outbox publication.
/// Belt-and-suspenders: also calls <see cref="IHybridCache.RemoveByTagAsync"/> for each user
/// currently assigned this role so their local cache entries are evicted immediately.
/// Throws <see cref="NotFoundException"/> when the role does not exist, or
/// <see cref="InvalidOperationException"/> when any permission name is not registered.
/// </summary>
public sealed class AssignPermissionsCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUserRoleAssignmentRepository assignmentRepository,
    IHybridCache cache
) : ICommandHandler<AssignPermissionsCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        AssignPermissionsCommand request,
        CancellationToken cancellationToken
    )
    {
        var role =
            await roleRepository.FindByIdAsync(request.RoleId, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.RoleId);

        // Clearing all permissions is a valid operation — skip catalogue lookup
        if (request.PermissionNames.Count == 0)
        {
            role.SetPermissions([]);
            await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            await InvalidateCacheForRoleUsersAsync(role, cancellationToken);
            return default;
        }

        // Validate all requested permission names exist in the global catalogue
        var permissions = await permissionRepository.GetByNamesAsync(
            request.PermissionNames,
            cancellationToken
        );

        var foundNames = permissions
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingNames = request.PermissionNames.Where(n => !foundNames.Contains(n)).ToList();

        if (missingNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Unknown permission(s): {string.Join(", ", missingNames)}. "
                    + "Register them first via the permission registration endpoint."
            );
        }

        // SetPermissions raises RolePermissionsChangedEvent for outbox publication.
        role.SetPermissions(permissions.Select(p => p.Id));
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // Belt-and-suspenders: immediately evict each affected user's local cache entry.
        await InvalidateCacheForRoleUsersAsync(role, cancellationToken);

        return default;
    }

    /// <summary>
    /// Enumerates all assignments for the given role and invalidates the hybrid cache tag
    /// for each affected user within their respective school.
    /// </summary>
    private async Task InvalidateCacheForRoleUsersAsync(
        Role role,
        CancellationToken cancellationToken
    )
    {
        // GetAllByRoleIdAsync bypasses tenant filter so we reach all schools' assignments.
        var assignments = await assignmentRepository.GetAllByRoleIdAsync(
            role.Id,
            cancellationToken
        );

        foreach (var assignment in assignments)
        {
            var tag = $"user:{assignment.ProfileId}:{role.SchoolId}";
            await cache.RemoveByTagAsync(tag, cancellationToken);
        }
    }
}
