namespace Edvantix.Organizations.Features.Groups.CreateGroup;

/// <summary>
/// Handles group creation by injecting the current tenant's SchoolId from <see cref="ITenantContext"/>.
/// </summary>
public sealed class CreateGroupCommandHandler(
    IGroupRepository groupRepository,
    ITenantContext tenantContext
) : ICommandHandler<CreateGroupCommand, Guid>
{
    /// <inheritdoc/>
    public async ValueTask<Guid> Handle(
        CreateGroupCommand request,
        CancellationToken cancellationToken
    )
    {
        var group = new Group(
            request.Name,
            tenantContext.SchoolId,
            request.MaxCapacity,
            request.Color
        );
        groupRepository.Add(group);
        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return group.Id;
    }
}
