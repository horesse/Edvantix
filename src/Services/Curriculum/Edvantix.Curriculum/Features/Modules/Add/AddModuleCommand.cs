using Edvantix.Chassis.CQRS;
using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

namespace Edvantix.Curriculum.Features.Modules.Add;

/// <summary>Добавляет модуль в курс. Возвращает идентификатор нового модуля.</summary>
[Transactional]
public sealed record AddModuleCommand(Guid CourseId, string Name, string? Summary, short Weeks)
    : ICommand<Guid>;

internal sealed class AddModuleCommandHandler(
    ITenantContext tenantContext,
    ICourseRepository repository
) : ICommandHandler<AddModuleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        AddModuleCommand command,
        CancellationToken cancellationToken
    )
    {
        var course =
            await repository.GetByIdForWriteAsync(command.CourseId, cancellationToken)
            ?? throw NotFoundException.For<Course>(command.CourseId);

        if (course.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Course>(command.CourseId);

        var module = course.AddModule(command.Name, command.Summary, command.Weeks);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return module.Id;
    }
}
