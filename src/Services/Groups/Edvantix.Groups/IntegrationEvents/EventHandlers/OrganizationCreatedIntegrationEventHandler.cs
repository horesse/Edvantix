using Edvantix.Contracts;
using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.IntegrationEvents.EventHandlers;

/// <summary>
/// При создании организации сидирует 8 базовых уровней (A1–C1, JR, TN, PR) в Groups-сервисе.
/// Уровни активны, доступны для редактирования и удаления через UI настроек.
/// </summary>
public sealed class OrganizationCreatedIntegrationEventHandler(ILevelRepository repository)
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

    public async Task Handle(
        OrganizationCreatedIntegrationEvent @event,
        CancellationToken cancellationToken
    )
    {
        foreach (var (code, name, tone, order) in Defaults)
        {
            var level = new Level(
                @event.OrganizationId,
                LevelCode.From(code),
                name,
                null,
                tone,
                order
            );
            await repository.AddAsync(level, cancellationToken);
        }

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
