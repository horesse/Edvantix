using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Modules.Delete;

/// <summary>Удаляет модуль из курса и переиндексирует оставшиеся модули.</summary>
[Transactional]
public sealed record DeleteModuleCommand(Guid CourseId, Guid ModuleId) : ICommand;

internal sealed class DeleteModuleCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<DeleteModuleCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteModuleCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdForWriteAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        course.DeleteModule(command.ModuleId);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
