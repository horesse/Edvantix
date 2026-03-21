namespace Edvantix.Organizations.Features.Groups.UpdateGroup;

/// <summary>Updates a group's properties or throws <see cref="NotFoundException"/> when not found.</summary>
public sealed class UpdateGroupCommandHandler(IGroupRepository groupRepository)
    : ICommandHandler<UpdateGroupCommand, Unit>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        UpdateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var group =
            await groupRepository.FindByIdAsync(request.Id, cancellationToken)
            ?? throw NotFoundException.For<Group>(request.Id);

        group.Update(request.Name, request.MaxCapacity, request.Color);
        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return default;
    }
}
