using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// При создании организации сидирует 8 базовых уровней (A1–C1, JR, TN, PR).
/// Уровни обычные — IsActive=true, не системные, доступны для редактирования и удаления.
/// </summary>
internal sealed class OrganizationCreatedSeedLevelsHandler(ILevelRepository levels)
    : INotificationHandler<OrganizationCreatedDomainEvent>
{
    private static readonly (string Code, string Name, LevelTone Tone, short Order)[] Defaults =
    [
        ("A1", "A1 — Начальный", LevelTone.Teal, 10),
        ("A2", "A2 — Базовый", LevelTone.Teal, 20),
        ("B1", "B1 — Средний", LevelTone.Blue, 30),
        ("B2", "B2 — Продвинутый", LevelTone.Blue, 40),
        ("C1", "C1 — Высокий", LevelTone.Indigo, 50),
        ("JR", "Дети 7–10 лет", LevelTone.Amber, 60),
        ("TN", "Подростки 11–14 лет", LevelTone.Amber, 70),
        ("PR", "Подготовка к экзаменам", LevelTone.Violet, 80),
    ];

    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        foreach (var (code, name, tone, order) in Defaults)
        {
            var level = new Level(
                notification.OrganizationId,
                LevelCode.From(code),
                name,
                null,
                tone,
                order
            );
            await levels.AddAsync(level, cancellationToken);
        }

        await levels.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
