namespace Edvantix.Organizations.Features.Roles.GetRoleById;

/// <summary>Single role detail — name only, without permissions list.</summary>
public sealed record RoleDetailItem(Guid Id, string Name);

/// <summary>Query that returns a single role by its identifier.</summary>
public sealed class GetRoleByIdQuery : IQuery<RoleDetailItem>
{
    public required Guid Id { get; init; }
}

/// <summary>Resolves a role by ID, throwing <see cref="NotFoundException"/> when not found.</summary>
public sealed class GetRoleByIdQueryHandler(IRoleRepository roleRepository)
    : IQueryHandler<GetRoleByIdQuery, RoleDetailItem>
{
    /// <inheritdoc/>
    public async ValueTask<RoleDetailItem> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var role = await roleRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.Id);
        return new RoleDetailItem(role.Id, role.Name);
    }
}
