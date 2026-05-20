using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Groups.Grpc.Services.Courses;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Groups.Create;

[Transactional]
[RequirePermission(GroupPermissions.Create)]
public sealed record CreateGroupCommand(
    string Code,
    string Name,
    string Description,
    Guid LevelId,
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
    IGroupRepository repository,
    ICurriculumService curriculumService
) : ICommandHandler<CreateGroupCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateGroupCommand command,
        CancellationToken cancellationToken
    )
    {
        var courseId = command.CourseId.ToString();
        var course =
            await curriculumService.GetCourseByIdAsync(courseId, cancellationToken)
            ?? throw new NotFoundException($"Course {courseId} not found.");

        if (course.OrganizationId != tenantContext.OrganizationId.ToString())
            throw new ForbiddenException("Курс не принадлежит текущей организации.");

        var code = GroupCode.From(command.Code);

        var group = new Group(
            tenantContext.OrganizationId,
            code,
            command.Name,
            command.Description,
            command.LevelId,
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
