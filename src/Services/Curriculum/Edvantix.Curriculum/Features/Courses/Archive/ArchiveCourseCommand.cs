using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Courses.Archive;

/// <summary>Переводит курс в статус Archived.</summary>
[Transactional]
public sealed record ArchiveCourseCommand(Guid CourseId) : ICommand;

internal sealed class ArchiveCourseCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<ArchiveCourseCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveCourseCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        course.Archive();
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
