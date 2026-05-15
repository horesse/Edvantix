using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Members.Remove;

[Transactional]
[RequirePermission(GroupPermissions.Members)]
public sealed record RemoveGroupMemberCommand(
    Guid GroupId,
    Guid MemberId,
    DateOnly ExitedAt,
    string? Reason
) : ICommand;

internal sealed class RemoveGroupMemberCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<RemoveGroupMemberCommand>
{
    public async ValueTask<Unit> Handle(
        RemoveGroupMemberCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(command.GroupId, cancellationToken);
        Guard.Against.NotFound(group, command.GroupId);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        var member = group.Members.FirstOrDefault(m =>
            m.Id == command.MemberId && m.ExitedAt is null
        );

        if (member is null)
            throw new NotFoundException(
                $"Активный участник {command.MemberId} не найден в группе."
            );

        if (command.ExitedAt < member.JoinedAt)
            throw new ArgumentException(
                $"Дата выхода не может быть раньше даты вступления ({member.JoinedAt:dd.MM.yyyy}).",
                nameof(command.ExitedAt)
            );

        group.RemoveMember(command.MemberId, command.ExitedAt, command.Reason);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
