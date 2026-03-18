namespace Edvantix.Organizations.Features.Roles.UpdateRole;

/// <summary>Command to rename an existing role.</summary>
public sealed class UpdateRoleCommand : ICommand<Unit>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

/// <summary>Renames the role or throws <see cref="NotFoundException"/> when not found.</summary>
public sealed class UpdateRoleCommandHandler(IRoleRepository roleRepository)
    : ICommandHandler<UpdateRoleCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        UpdateRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role =
            await roleRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.Id);

        role.UpdateName(request.Name);
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return default;
    }
}
