using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Groups.Domain.Permissions;

namespace Edvantix.Groups.Features.Levels.Delete;

/// <summary>
/// Удалить уровень из справочника организации (мягкое удаление).
/// Уровень нельзя удалить, если он используется хотя бы одной группой.
/// </summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record DeleteLevelCommand(Guid Id) : ICommand;

internal sealed class DeleteLevelCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<DeleteLevelCommand>
{
    public async ValueTask<Unit> Handle(
        DeleteLevelCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        var isUsed = await repository.IsUsedByGroupsAsync(command.Id, cancellationToken);

        if (isUsed)
            throw new InvalidOperationException(
                "Уровень нельзя удалить: он используется в одной или нескольких группах."
            );

        level.Delete();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
