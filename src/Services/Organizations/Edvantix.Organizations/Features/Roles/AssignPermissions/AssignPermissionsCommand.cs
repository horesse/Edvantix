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
/// Throws <see cref="NotFoundException"/> when the role does not exist, or
/// <see cref="InvalidOperationException"/> when any permission name is not registered.
/// </summary>
public sealed class AssignPermissionsCommandHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository
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
            return default;
        }

        // Validate all requested permission names exist in the global catalogue
        var permissions = await permissionRepository.GetByNamesAsync(
            request.PermissionNames,
            cancellationToken
        );

        var foundNames = permissions.Select(p => p.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingNames = request.PermissionNames
            .Where(n => !foundNames.Contains(n))
            .ToList();

        if (missingNames.Count > 0)
        {
            throw new InvalidOperationException(
                $"Unknown permission(s): {string.Join(", ", missingNames)}. "
                    + "Register them first via the permission registration endpoint."
            );
        }

        role.SetPermissions(permissions.Select(p => p.Id));
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return default;
    }
}
