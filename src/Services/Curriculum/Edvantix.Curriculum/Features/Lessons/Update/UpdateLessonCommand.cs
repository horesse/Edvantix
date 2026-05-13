using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Lessons.Update;

/// <summary>Обновляет поля урока.</summary>
[Transactional]
public sealed record UpdateLessonCommand(
    Guid LessonId,
    string Title,
    LessonType Type,
    short Minutes,
    string[] Objectives
) : ICommand;

internal sealed class UpdateLessonCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<UpdateLessonCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateLessonCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByLessonIdAsync(command.LessonId, cancellationToken)
            ?? throw NotFoundException.For<Lesson>(command.LessonId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Lesson>(command.LessonId);

        course.UpdateLesson(
            command.LessonId,
            command.Title,
            command.Type,
            command.Minutes,
            command.Objectives
        );
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
