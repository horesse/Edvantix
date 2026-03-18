namespace Edvantix.Organizations.Features.Roles.CreateRole;

/// <summary>Command to create a new named role scoped to the current tenant (school).</summary>
public sealed class CreateRoleCommand : ICommand<Guid>
{
    public required string Name { get; init; }
}

/// <summary>Handles role creation, injecting the current tenant's SchoolId from <see cref="ITenantContext"/>.</summary>
public sealed class CreateRoleCommandHandler(
    IRoleRepository roleRepository,
    ITenantContext tenantContext
) : ICommandHandler<CreateRoleCommand, Guid>
{
    /// <inheritdoc/>
    public async ValueTask<Guid> Handle(
        CreateRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role = new Role(request.Name, tenantContext.SchoolId);
        await roleRepository.AddAsync(role, cancellationToken);
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return role.Id;
    }
}
