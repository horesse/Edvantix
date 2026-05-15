using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Members.Add;

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
    IGroupRepository groupRepository,
    IOrganizationMemberRepository memberRepository
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

        var roleId = await memberRepository.GetActiveMemberRoleIdAsync(
            tenantContext.OrganizationId,
            command.ProfileId,
            cancellationToken
        );

        if (roleId is null)
            throw new NotFoundException(
                $"Профиль {command.ProfileId} не является активным участником организации."
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
