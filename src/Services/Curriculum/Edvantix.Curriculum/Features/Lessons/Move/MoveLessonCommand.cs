using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Lessons.Move;

/// <summary>Перемещает урок на новую позицию в рамках его модуля.</summary>
[Transactional]
public sealed record MoveLessonCommand(Guid LessonId, short NewPosition) : ICommand;

internal sealed class MoveLessonCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<MoveLessonCommand>
{
    public async ValueTask<Unit> Handle(
        MoveLessonCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByLessonIdAsync(command.LessonId, cancellationToken)
            ?? throw NotFoundException.For<Lesson>(command.LessonId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Lesson>(command.LessonId);

        course.MoveLesson(command.LessonId, command.NewPosition);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
