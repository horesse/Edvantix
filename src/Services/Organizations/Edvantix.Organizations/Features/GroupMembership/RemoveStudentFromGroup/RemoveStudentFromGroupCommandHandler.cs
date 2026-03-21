namespace Edvantix.Organizations.Features.GroupMembership.RemoveStudentFromGroup;

/// <summary>
/// Handles removing a student from a group.
/// The operation is idempotent: if the student is not a member, the call succeeds silently.
/// </summary>
public sealed class RemoveStudentFromGroupCommandHandler(IGroupRepository groupRepository)
    : ICommandHandler<RemoveStudentFromGroupCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        RemoveStudentFromGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var group =
            await groupRepository.FindAsync(
                new GroupByIdSpecification(request.GroupId, includeMembers: true),
                cancellationToken
            ) ?? throw NotFoundException.For<Group>(request.GroupId);

        // RemoveMember is a no-op if the student is not a member.
        group.RemoveMember(request.ProfileId);

        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
