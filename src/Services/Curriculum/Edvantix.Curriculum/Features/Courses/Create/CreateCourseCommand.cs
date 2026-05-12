using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Courses.Create;

/// <summary>
/// Создаёт новый курс в статусе <see cref="CourseStatus.Draft"/>.
/// Возвращает идентификатор созданного курса.
/// </summary>
[Transactional]
public sealed record CreateCourseCommand(
    string Code,
    string Name,
    CourseSubject Subject,
    string Level,
    short DurationWeeks,
    Guid OwnerMemberId,
    string? Description = null
) : ICommand<Guid>;

internal sealed class CreateCourseCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<CreateCourseCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateCourseCommand command,
        CancellationToken cancellationToken
    )
    {
        var course = new Course(
            tenantContext.OrganizationId,
            command.Code,
            command.Name,
            command.Subject,
            command.Level,
            command.DurationWeeks,
            command.OwnerMemberId,
            command.Description
        );

        await repository.AddAsync(course, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return course.Id;
    }
}
