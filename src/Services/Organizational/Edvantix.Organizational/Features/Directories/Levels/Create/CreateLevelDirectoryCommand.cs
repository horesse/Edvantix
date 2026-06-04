using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.Create;

/// <summary>Создать запись справочника «Уровни».</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record CreateLevelDirectoryCommand(
    string Name,
    string Code,
    short Order,
    string? Description,
    LevelTone Tone = LevelTone.Indigo
) : ICommand<LevelDirectoryDto>;

internal sealed class CreateLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<CreateLevelDirectoryCommand, LevelDirectoryDto>
{
    public async ValueTask<LevelDirectoryDto> Handle(
        CreateLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = new Level(
            tenantContext.OrganizationId,
            LevelCode.From(command.Code),
            command.Name,
            command.Description,
            command.Tone,
            command.Order
        );

        await repository.AddAsync(level, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return LevelDirectoryMapper.ToDto(level);
    }
}
