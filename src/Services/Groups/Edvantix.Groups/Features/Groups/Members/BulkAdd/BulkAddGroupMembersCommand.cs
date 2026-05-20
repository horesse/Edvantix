using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.Members.BulkAdd;

[Transactional]
[RequirePermission(GroupPermissions.Members)]
public sealed record BulkAddGroupMembersCommand(Guid GroupId, IReadOnlyList<BulkAddItem> Items)
    : ICommand<BulkAddResult>;

/// <summary>Элемент пакетного запроса на добавление участника.</summary>
public sealed record BulkAddItem(Guid ProfileId, GroupMemberRole Role, DateOnly JoinedAt);

/// <summary>Результат пакетного добавления участников.</summary>
public sealed record BulkAddResult(IReadOnlyList<Guid> Added, IReadOnlyList<BulkAddFailure> Failed);

/// <summary>Информация о неудачном добавлении участника в пакетной операции.</summary>
public sealed record BulkAddFailure(Guid ProfileId, string Reason);

internal sealed class BulkAddGroupMembersCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository groupRepository
) : ICommandHandler<BulkAddGroupMembersCommand, BulkAddResult>
{
    public async ValueTask<BulkAddResult> Handle(
        BulkAddGroupMembersCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        Guard.Against.NotFound(group, command.GroupId);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        var added = new List<Guid>();
        var failed = new List<BulkAddFailure>();

        foreach (var item in command.Items)
        {
            if (item.JoinedAt < group.StartDate)
            {
                failed.Add(
                    new BulkAddFailure(
                        item.ProfileId,
                        $"Дата вступления не может быть раньше даты начала группы ({group.StartDate:dd.MM.yyyy})."
                    )
                );
                continue;
            }

            try
            {
                var member = new GroupMember(
                    tenantContext.OrganizationId,
                    command.GroupId,
                    item.ProfileId,
                    item.Role,
                    item.JoinedAt
                );

                group.AddMember(member);
                added.Add(member.Id);
            }
            catch (InvalidOperationException ex)
            {
                failed.Add(new BulkAddFailure(item.ProfileId, ex.Message));
            }
        }

        if (added.Count > 0)
            await groupRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return new BulkAddResult(added, failed);
    }
}
