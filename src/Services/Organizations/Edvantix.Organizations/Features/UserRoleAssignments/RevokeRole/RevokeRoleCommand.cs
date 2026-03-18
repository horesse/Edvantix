namespace Edvantix.Organizations.Features.UserRoleAssignments.RevokeRole;

/// <summary>Command to revoke (hard-delete) a role assignment from a user profile.</summary>
public sealed class RevokeRoleCommand : ICommand
{
    /// <summary>Gets the profile (user) identifier from which to revoke the role.</summary>
    public required Guid ProfileId { get; init; }

    /// <summary>Gets the role identifier to revoke.</summary>
    public required Guid RoleId { get; init; }
}

/// <summary>
/// Handles role revocation by hard-deleting the <see cref="UserRoleAssignment"/>.
/// Throws <see cref="NotFoundException"/> if the assignment does not exist.
/// </summary>
public sealed class RevokeRoleCommandHandler(IUserRoleAssignmentRepository assignmentRepository)
    : ICommandHandler<RevokeRoleCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        RevokeRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var assignment =
            await assignmentRepository.FindAsync(
                request.ProfileId,
                request.RoleId,
                cancellationToken
            )
            ?? throw new NotFoundException(
                $"UserRoleAssignment for profileId={request.ProfileId}, roleId={request.RoleId} not found."
            );

        assignmentRepository.Remove(assignment);
        await assignmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
