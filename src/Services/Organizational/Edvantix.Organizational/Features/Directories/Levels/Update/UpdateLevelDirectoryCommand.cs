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
    short Order,
    string? Description
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

        // Tone preserves its existing value; Code is immutable.
        level.Update(command.Name, command.Description, level.Tone, command.Order);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return LevelDirectoryMapper.ToDto(level);
    }
}
