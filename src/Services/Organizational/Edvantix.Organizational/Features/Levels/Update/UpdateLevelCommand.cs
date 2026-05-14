using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Levels.Update;

/// <summary>Обновить данные уровня. Код уровня не изменяется.</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record UpdateLevelCommand(
    Guid Id,
    string Name,
    string? Description,
    LevelTone Tone,
    short SortOrder
) : ICommand;

internal sealed class UpdateLevelCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<UpdateLevelCommand>
{
    public async ValueTask<Unit> Handle(
        UpdateLevelCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        level.Update(command.Name, command.Description, command.Tone, command.SortOrder);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
