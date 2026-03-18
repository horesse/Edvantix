namespace Edvantix.Organizations.Features.Roles.DeleteRole;

/// <summary>Command to soft-delete a role. Fails with 409 if active user assignments exist.</summary>
public sealed class DeleteRoleCommand : ICommand<Unit>
{
    public required Guid Id { get; init; }
}

/// <summary>
/// Soft-deletes a role after confirming no active user assignments reference it.
/// Throws <see cref="NotFoundException"/> if the role does not exist,
/// or <see cref="InvalidOperationException"/> (maps to 409 Conflict) if assignments exist.
/// </summary>
public sealed class DeleteRoleCommandHandler(
    IRoleRepository roleRepository,
    IUserRoleAssignmentRepository assignmentRepository
) : ICommandHandler<DeleteRoleCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var role =
            await roleRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.Id);

        // Per locked decision: 409 Conflict if role has active assignments.
        // GlobalExceptionHandler maps InvalidOperationException -> 409.
        if (await assignmentRepository.ExistsByRoleIdAsync(request.Id, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Cannot delete role '{role.Name}' because it is assigned to users. Revoke all assignments first."
            );
        }

        role.Delete(); // soft-delete via ISoftDelete.IsDeleted = true
        await roleRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return default;
    }
}
