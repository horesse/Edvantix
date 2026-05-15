using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Grpc.Services.Courses;

namespace Edvantix.Organizational.Features.Groups.Update;

[Transactional]
[RequirePermission(GroupPermissions.Edit)]
public sealed record UpdateGroupCommand(
    Guid Id,
    string Name,
    string Description,
    Guid LevelId,
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
    IGroupRepository repository,
    ICurriculumService curriculumService
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

        // Validate CourseId only when it changes to avoid redundant gRPC call.
        if (command.CourseId != group.CourseId)
        {
            var courseId = command.CourseId.ToString();
            var course =
                await curriculumService.GetCourseByIdAsync(courseId, cancellationToken)
                ?? throw new NotFoundException($"Course {courseId} not found.");

            if (course.OrganizationId != tenantContext.OrganizationId.ToString())
                throw new ForbiddenException("Курс не принадлежит текущей организации.");
        }

        group.Update(
            command.Name,
            command.Description,
            command.LevelId,
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
