namespace Edvantix.Organizations.Features.Roles.GetRolePermissions;

/// <summary>Permission item returned for a role's permission list.</summary>
public sealed record RolePermissionItem(Guid Id, string Name);

/// <summary>Query that returns all permissions assigned to a role.</summary>
public sealed class GetRolePermissionsQuery : IQuery<List<RolePermissionItem>>
{
    public required Guid RoleId { get; init; }
}

/// <summary>
/// Resolves role permissions by fetching the role, extracting its permission IDs,
/// then fetching all permissions from the catalogue and filtering in-memory.
/// This is acceptable for v1 — the Permission catalogue will remain small (tens of entries).
/// If the catalogue grows significantly, add <c>GetByIdsAsync</c> to <see cref="IPermissionRepository"/>.
/// </summary>
public sealed class GetRolePermissionsQueryHandler(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository
) : IQueryHandler<GetRolePermissionsQuery, List<RolePermissionItem>>
{
    /// <inheritdoc/>
    public async ValueTask<List<RolePermissionItem>> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var role =
            await roleRepository.FindByIdAsync(request.RoleId, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.RoleId);

        var permissionIds = role.Permissions.Select(rp => rp.PermissionId).ToHashSet();
        if (permissionIds.Count == 0)
        {
            return [];
        }

        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);
        return allPermissions
            .Where(p => permissionIds.Contains(p.Id))
            .Select(p => new RolePermissionItem(p.Id, p.Name))
            .OrderBy(p => p.Name)
            .ToList();
    }
}
