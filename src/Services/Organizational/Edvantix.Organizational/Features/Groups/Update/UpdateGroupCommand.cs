using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Groups.Update;

[Transactional]
[RequirePermission(GroupPermissions.Edit)]
public sealed record UpdateGroupCommand(
    Guid Id,
    string Name,
    string Description,
    GroupLevel Level,
    Guid CourseId,
    Guid TeacherMemberId,
    GroupFormat Format,
    Guid? RoomId,
    OnlinePlatform? Platform,
    int Capacity,
    DateOnly EndDate
) : ICommand;

internal sealed class UpdateGroupCommandHandler(
    ITenantContext tenantContext,
    IGroupRepository repository
) : ICommandHandler<UpdateGroupCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateGroupCommand command,
        CancellationToken cancellationToken
    )
    {
        var group = await repository.GetByIdAsync(command.Id, cancellationToken);
        Guard.Against.NotFound(group, command.Id);

        if (group.OrganizationId != tenantContext.OrganizationId)
            throw new ForbiddenException("Группа не принадлежит текущей организации.");

        group.Update(
            command.Name,
            command.Description,
            command.Level,
            command.CourseId,
            command.TeacherMemberId,
            command.Format,
            command.RoomId,
            command.Platform,
            command.Capacity,
            command.EndDate
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
