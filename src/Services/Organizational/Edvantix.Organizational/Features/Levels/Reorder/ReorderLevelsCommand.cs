using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Levels.Reorder;

/// <summary>Элемент порядка уровня в операции переупорядочивания.</summary>
/// <param name="Id">Идентификатор уровня.</param>
/// <param name="SortOrder">Новый порядковый номер.</param>
public sealed record LevelOrderItem(Guid Id, short SortOrder);

/// <summary>
/// Переупорядочить уровни справочника — задать новый <c>SortOrder</c> для набора уровней.
/// Все переданные идентификаторы должны принадлежать текущей организации.
/// </summary>
[Transactional]
[RequirePermission(LevelPermissions.Manage)]
public sealed record ReorderLevelsCommand(IReadOnlyList<LevelOrderItem> Items) : ICommand;

internal sealed class ReorderLevelsCommandHandler(
    ITenantContext tenantContext,
    ILevelRepository repository
) : ICommandHandler<ReorderLevelsCommand>
{
    public async ValueTask<Unit> Handle(
        ReorderLevelsCommand command,
        CancellationToken cancellationToken
    )
    {
        var ids = command.Items.Select(i => i.Id).ToList();
        var levels = await repository.GetByIdsAsync(ids, cancellationToken);

        // Проверить, что все уровни принадлежат текущей организации
        var foreignLevel = levels.FirstOrDefault(l =>
            l.OrganizationId != tenantContext.OrganizationId
        );

        if (foreignLevel is not null)
            throw NotFoundException.For<Level>(foreignLevel.Id);

        if (levels.Count != ids.Count)
        {
            var missingId = ids.FirstOrDefault(id => levels.All(l => l.Id != id));
            throw NotFoundException.For<Level>(missingId);
        }

        // Проверить уникальность SortOrder в переданных данных
        var hasDuplicateSortOrder = command.Items
            .GroupBy(i => i.SortOrder)
            .Any(g => g.Count() > 1);

        if (hasDuplicateSortOrder)
            throw new InvalidOperationException(
                "Переданные порядковые номера содержат дубликаты."
            );

        var levelMap = levels.ToDictionary(l => l.Id);

        foreach (var item in command.Items)
        {
            levelMap[item.Id].SetSortOrder(item.SortOrder);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
