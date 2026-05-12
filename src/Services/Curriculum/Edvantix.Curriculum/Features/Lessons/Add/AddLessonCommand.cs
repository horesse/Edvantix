using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Features.Lessons.Add;

/// <summary>Добавляет урок в модуль. Возвращает идентификатор нового урока.</summary>
[Transactional]
public sealed record AddLessonCommand(
    Guid ModuleId,
    string Title,
    LessonType Type,
    short Minutes,
    string[] Objectives
) : ICommand<Guid>;

internal sealed class AddLessonCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<AddLessonCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddLessonCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByModuleIdAsync(command.ModuleId, cancellationToken)
            ?? throw NotFoundException.For<Module>(command.ModuleId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Module>(command.ModuleId);

        var lesson = course.AddLesson(
            command.ModuleId,
            command.Title,
            command.Type,
            command.Minutes,
            command.Objectives
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return lesson.Id;
    }
}
