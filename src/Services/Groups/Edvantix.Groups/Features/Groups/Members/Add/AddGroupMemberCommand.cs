using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.Members.Add;

[Transactional]
[RequirePermission(GroupPermissions.Members)]
public sealed record AddGroupMemberCommand(
    Guid GroupId,
    Guid ProfileId,
    GroupMemberRole Role,
    DateOnly JoinedAt
) : ICommand<Guid>;

internal sealed class AddGroupMemberCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository groupRepository
) : ICommandHandler<AddGroupMemberCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddGroupMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        Guard.Against.NotFound(group, command.GroupId);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        if (command.JoinedAt < group.StartDate)
            throw new ArgumentException(
                $"Дата вступления не может быть раньше даты начала группы ({group.StartDate:dd.MM.yyyy}).",
                nameof(command.JoinedAt)
            );

        var member = new GroupMember(
            tenantContext.OrganizationId,
            command.GroupId,
            command.ProfileId,
            command.Role,
            command.JoinedAt
        );

        group.AddMember(member);

        await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return member.Id;
    }
}
