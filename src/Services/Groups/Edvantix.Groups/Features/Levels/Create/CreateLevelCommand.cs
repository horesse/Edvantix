using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Groups.Domain.Permissions;

namespace Edvantix.Groups.Features.Levels.Create;

/// <summary>Создать новый уровень в справочнике организации.</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record CreateLevelCommand(
    string Code,
    string Name,
    string? Description,
    LevelTone Tone,
    short SortOrder
) : ICommand<Guid>;

internal sealed class CreateLevelCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<CreateLevelCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateLevelCommand command,
        CancellationToken cancellationToken
    )
    {
        var codeVo = LevelCode.From(command.Code);

        var isDuplicate = await repository.ExistsWithCodeAsync(
            tenantContext.OrganizationId,
            codeVo.Value,
            cancellationToken
        );

        if (isDuplicate)
            throw new InvalidOperationException(
                $"Уровень с кодом '{codeVo.Value}' уже существует в организации."
            );

        var level = new Level(
            tenantContext.OrganizationId,
            codeVo,
            command.Name,
            command.Description,
            command.Tone,
            command.SortOrder
        );

        await repository.AddAsync(level, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return level.Id;
    }
}
