using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Create;

[Transactional]
[RequirePermission(GroupPermissions.Create)]
public sealed record CreateGroupCommand(
    string Code,
    string Name,
    string Description,
    GroupLevel Level,
    Guid CourseId,
    Guid TeacherMemberId,
    GroupFormat Format,
    Guid? RoomId,
    OnlinePlatform? Platform,
    int Capacity,
    DateOnly StartDate,
    DateOnly EndDate
) : ICommand<Guid>;

internal sealed class CreateGroupCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<CreateGroupCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateGroupCommand command,
        CancellationToken cancellationToken
    )
    {
        var code = GroupCode.From(command.Code);

        var group = new Group(
            tenantContext.OrganizationId,
            code,
            command.Name,
            command.Description,
            command.Level,
            command.CourseId,
            command.TeacherMemberId,
            command.Format,
            command.RoomId,
            command.Platform,
            command.Capacity,
            command.StartDate,
            command.EndDate
        );

        await repository.AddAsync(group, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return group.Id;
    }
}
