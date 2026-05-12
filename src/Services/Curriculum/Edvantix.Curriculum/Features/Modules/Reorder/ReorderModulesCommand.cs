using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Modules.Reorder;

/// <summary>Переупорядочивает модули курса.</summary>
[Transactional]
public sealed record ReorderModulesCommand(Guid CourseId, IReadOnlyList<Guid> OrderedModuleIds)
    : ICommand;

internal sealed class ReorderModulesCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<ReorderModulesCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderModulesCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdForWriteAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        course.ReorderModules(command.OrderedModuleIds);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
