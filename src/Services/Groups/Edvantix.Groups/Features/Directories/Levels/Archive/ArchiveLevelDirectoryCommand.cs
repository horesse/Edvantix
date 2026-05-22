using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Levels.Archive;

/// <summary>Деактивировать уровень (перевод в архив справочника).</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record ArchiveLevelDirectoryCommand(Guid Id) : ICommand;

internal sealed class ArchiveLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<ArchiveLevelDirectoryCommand>
{
    public async ValueTask<Unit> Handle(
        ArchiveLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        // Idempotent: повторный архив не меняет состояние.
        level.Deactivate();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
