using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Levels.Deactivate;

/// <summary>Деактивировать уровень — он не будет доступен для выбора в новых группах.</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record DeactivateLevelCommand(Guid Id) : ICommand;

internal sealed class DeactivateLevelCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<DeactivateLevelCommand>
{
    public async ValueTask<Unit> Handle(
        DeactivateLevelCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        level.Deactivate();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
