using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Levels.Activate;

/// <summary>Активировать уровень — он станет доступен для выбора в группах.</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record ActivateLevelCommand(Guid Id) : ICommand;

internal sealed class ActivateLevelCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<ActivateLevelCommand>
{
    public async ValueTask<Unit> Handle(
        ActivateLevelCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        level.Activate();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
