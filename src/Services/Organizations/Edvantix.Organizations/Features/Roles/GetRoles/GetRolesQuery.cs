namespace Edvantix.Organizations.Features.Roles.GetRoles;

/// <summary>Lightweight role summary — name only, no permissions included.</summary>
public sealed record RoleListItem(Guid Id, string Name);

/// <summary>Query that returns all roles for the current tenant.</summary>
public sealed class GetRolesQuery : IQuery<List<RoleListItem>>;

/// <summary>Returns all roles ordered by name; tenant filter is applied by the DbContext query filter.</summary>
public sealed class GetRolesQueryHandler(IRoleRepository roleRepository)
    : IQueryHandler<GetRolesQuery, List<RoleListItem>>
{
    /// <inheritdoc/>
    public async ValueTask<List<RoleListItem>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken
    )
    {
        var roles = await roleRepository.GetAllAsync(cancellationToken);
        return roles.Select(r => new RoleListItem(r.Id, r.Name)).ToList();
    }
}
