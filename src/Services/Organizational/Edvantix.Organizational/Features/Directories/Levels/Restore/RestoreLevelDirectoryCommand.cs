using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.Restore;

/// <summary>Активировать уровень (восстановить из архива справочника).</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record RestoreLevelDirectoryCommand(Guid Id) : ICommand;

internal sealed class RestoreLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<RestoreLevelDirectoryCommand>
{
    public async ValueTask<Unit> Handle(
        RestoreLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        // Idempotent: повторное восстановление не меняет состояние.
        level.Activate();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
