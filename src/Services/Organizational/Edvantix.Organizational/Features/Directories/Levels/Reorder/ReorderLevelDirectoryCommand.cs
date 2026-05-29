using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Levels.Reorder;

/// <summary>
/// Переупорядочить уровни согласно переданному списку идентификаторов.
/// <para>
/// Использует двухфазный подход для обхода уникального ограничения
/// <c>SortOrder</c> per-org: сначала все SortOrder сдвигаются в временный диапазон (+10000),
/// затем выставляются финальные значения 0..n-1.
/// </para>
/// </summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record ReorderLevelDirectoryCommand(IReadOnlyList<Guid> OrderedIds) : ICommand;

internal sealed class ReorderLevelDirectoryCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<ReorderLevelDirectoryCommand>
{
    private const short TempOffset = 10_000;

    public async ValueTask<Unit> Handle(
        ReorderLevelDirectoryCommand command,
        CancellationToken cancellationToken
    )
    {
        var allLevels = await repository.ListAsync(
            new LevelReorderSpec(tenantContext.OrganizationId),
            cancellationToken
        );

        var lookup = allLevels.ToDictionary(l => l.Id);

        // Определяем уровни из запроса, принадлежащие этой организации.
        var toReorder = command
            .OrderedIds.Select((id, index) => (id, index))
            .Where(t => lookup.ContainsKey(t.id))
            .ToList();

        // Фаза 1: сдвигаем SortOrder во временный диапазон, чтобы избежать коллизий.
        foreach (var (id, index) in toReorder)
            lookup[id].SetSortOrder((short)(TempOffset + index));

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // Фаза 2: выставляем финальные значения 0..n-1.
        foreach (var (id, index) in toReorder)
            lookup[id].SetSortOrder((short)index);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
