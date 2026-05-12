using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Lessons.Publish;

/// <summary>Публикует урок (переводит в статус Published).</summary>
[Transactional]
public sealed record PublishLessonCommand(Guid LessonId) : ICommand;

internal sealed class PublishLessonCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<PublishLessonCommand>
{
    public async ValueTask<Unit> Handle(
        PublishLessonCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByLessonIdAsync(command.LessonId, cancellationToken)
            ?? throw NotFoundException.For<Lesson>(command.LessonId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Lesson>(command.LessonId);

        course.PublishLesson(command.LessonId);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
