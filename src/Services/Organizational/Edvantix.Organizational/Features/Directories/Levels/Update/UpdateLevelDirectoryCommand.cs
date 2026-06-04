using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.Update;

/// <summary>Обновить запись справочника «Уровни».</summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record UpdateLevelDirectoryCommand(
    Guid Id,
    string Name,
    string Code,
    short Order,
    string? Description,
    LevelTone Tone = LevelTone.Indigo
) : ICommand<LevelDirectoryDto>;

internal sealed class UpdateLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<UpdateLevelDirectoryCommand, LevelDirectoryDto>
{
    public async ValueTask<LevelDirectoryDto> Handle(
        UpdateLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var level = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (level is null || level.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Level>(command.Id);

        level.Update(command.Name, command.Description, command.Tone, command.Order);

        var newCode = LevelCode.From(command.Code);
        if (newCode.Value != level.Code.Value)
            level.ChangeCode(newCode);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return LevelDirectoryMapper.ToDto(level);
    }
}
