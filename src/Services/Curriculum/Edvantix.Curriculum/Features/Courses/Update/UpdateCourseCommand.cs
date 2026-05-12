using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Courses.Update;

/// <summary>Обновляет основные поля курса (кроме Code и Subject).</summary>
[Transactional]
public sealed record UpdateCourseCommand(
    Guid CourseId,
    string Name,
    string? Description,
    string Level,
    short DurationWeeks,
    string? CoverInitials = null
) : ICommand;

internal sealed class UpdateCourseCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<UpdateCourseCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateCourseCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        course.Update(
            command.Name,
            command.Description,
            command.Level,
            command.DurationWeeks,
            command.CoverInitials
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Unit.Value;
    }
}
