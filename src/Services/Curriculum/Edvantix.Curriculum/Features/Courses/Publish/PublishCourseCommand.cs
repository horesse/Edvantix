using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Courses.Publish;

/// <summary>Переводит курс в статус Active (публикует).</summary>
[Transactional]
public sealed record PublishCourseCommand(Guid CourseId) : ICommand;

internal sealed class PublishCourseCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<PublishCourseCommand>
{
    public async ValueTask<Unit> Handle(
        PublishCourseCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdForWriteAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        course.Publish();
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
