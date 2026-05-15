using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Members.BulkAdd;

[Transactional]
[RequirePermission(GroupPermissions.Members)]
public sealed record BulkAddGroupMembersCommand(
    Guid GroupId,
    IReadOnlyList<BulkAddItem> Items
) : ICommand<BulkAddResult>;

/// <summary>Элемент пакетного запроса на добавление участника.</summary>
public sealed record BulkAddItem(Guid ProfileId, GroupMemberRole Role, DateOnly JoinedAt);

/// <summary>Результат пакетного добавления участников.</summary>
public sealed record BulkAddResult(
    IReadOnlyList<Guid> Added,
    IReadOnlyList<BulkAddFailure> Failed
);

/// <summary>Информация о неудачном добавлении участника в пакетной операции.</summary>
public sealed record BulkAddFailure(Guid ProfileId, string Reason);

internal sealed class BulkAddGroupMembersCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository groupRepository,
    IOrganizationMemberRepository memberRepository
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
            var failureReason = await ValidateItemAsync(item, group, cancellationToken);

            if (failureReason is not null)
            {
                failed.Add(new BulkAddFailure(item.ProfileId, failureReason));
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

    private async Task<string?> ValidateItemAsync(
        BulkAddItem item,
        Group group,
        CancellationToken cancellationToken
    )
    {
        if (item.JoinedAt < group.StartDate)
            return $"Дата вступления не может быть раньше даты начала группы ({group.StartDate:dd.MM.yyyy}).";

        var roleId = await memberRepository.GetActiveMemberRoleIdAsync(
            tenantContext.OrganizationId,
            item.ProfileId,
            cancellationToken
        );

        if (roleId is null)
            return $"Профиль {item.ProfileId} не является активным участником организации.";

        return null;
    }
}
