using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.LessonTypes.Archive;

/// <summary>Архивировать тип занятия (идемпотентная операция).</summary>
[Transactional]
[RequirePermission(LessonTypePermissions.Manage)]
public sealed record ArchiveLessonTypeCommand(Guid Id) : ICommand;

internal sealed class ArchiveLessonTypeCommandHandler(
    ITenantContext tenantContext,
    ILessonTypeRepository repository
) : ICommandHandler<ArchiveLessonTypeCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveLessonTypeCommand command,
        CancellationToken cancellationToken
    )
    {
        var lessonType = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (lessonType is null || lessonType.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<LessonType>(command.Id);

        lessonType.Archive(Guid.Empty);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
