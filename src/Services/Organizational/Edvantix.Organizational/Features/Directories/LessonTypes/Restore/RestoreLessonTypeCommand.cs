using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.LessonTypeAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.LessonTypes.Restore;

/// <summary>Восстановить тип занятия из архива (идемпотентная операция).</summary>
[Transactional]
[RequirePermission(LessonTypePermissions.Manage)]
public sealed record RestoreLessonTypeCommand(Guid Id) : ICommand;

internal sealed class RestoreLessonTypeCommandHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : ICommandHandler<RestoreLessonTypeCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreLessonTypeCommand command,
        CancellationToken cancellationToken
    )
    {
        var lessonType = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (lessonType is null || lessonType.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LessonType>(command.Id);

        lessonType.Restore(Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
