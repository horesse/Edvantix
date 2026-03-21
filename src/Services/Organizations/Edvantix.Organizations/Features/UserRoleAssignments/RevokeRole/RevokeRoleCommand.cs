using Edvantix.Chassis.Caching;

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
/// Calls <c>Revoke()</c> on the aggregate before removal so the domain event
/// (<see cref="Edvantix.Organizations.Infrastructure.EventServices.Events.UserRoleRevokedEvent"/>)
/// is dispatched via <c>SaveEntitiesAsync</c>, publishing the outbox integration event.
/// Belt-and-suspenders: also calls <see cref="IHybridCache.RemoveByTagAsync"/> to evict the
/// local hybrid cache entry immediately without waiting for the outbox consumer.
/// Throws <see cref="NotFoundException"/> if the assignment does not exist.
/// </summary>
public sealed class RevokeRoleCommandHandler(
    IUserRoleAssignmentRepository assignmentRepository,
    IHybridCache cache
) : ICommandHandler<RevokeRoleCommand>
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

        // Raise domain event before removal so SaveEntitiesAsync dispatches it via the outbox.
        assignment.Revoke();

        assignmentRepository.Remove(assignment);
        await assignmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // Belt-and-suspenders: immediately evict local hybrid cache for this user's school.
        var tag = $"user:{request.ProfileId}:{assignment.SchoolId}";
        await cache.RemoveByTagAsync(tag, cancellationToken);

        return Unit.Value;
    }
}
