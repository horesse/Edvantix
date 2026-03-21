namespace Edvantix.Organizations.Features.Groups.DeleteGroup;

/// <summary>
/// Soft-deletes a group by calling <see cref="Group.Delete"/>.
/// Throws <see cref="NotFoundException"/> if the group does not exist.
/// </summary>
public sealed class DeleteGroupCommandHandler(IGroupRepository groupRepository)
    : ICommandHandler<DeleteGroupCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        DeleteGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var group =
            await groupRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For<Group>(request.Id);

        // Soft-delete per D-02: IsDeleted flag rather than physical row removal.
        group.Delete();
        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
