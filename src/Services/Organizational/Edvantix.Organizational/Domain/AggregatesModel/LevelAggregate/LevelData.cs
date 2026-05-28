namespace Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

[ExcludeFromCodeCoverage]
internal sealed class LevelData : List<Level>
{
    public LevelData(Guid organizationId)
    {
        AddRange([
            CreateLevel(
                organizationId,
                LevelCode.From("A1"),
                "A1 — Начальный",
                "",
                LevelTone.Teal,
                1
            ),
            CreateLevel(
                organizationId,
                LevelCode.From("A2"),
                "A2 — Базовый",
                "",
                LevelTone.Teal,
                2
            ),
            CreateLevel(
                organizationId,
                LevelCode.From("B1"),
                "B1 — Средний",
                "",
                LevelTone.Blue,
                3
            ),
            CreateLevel(
                organizationId,
                LevelCode.From("B2"),
                "B2 — Продвинутый",
                "",
                LevelTone.Blue,
                4
            ),
            CreateLevel(
                organizationId,
                LevelCode.From("C1"),
                "C1 — Высокий",
                "",
                LevelTone.Indigo,
                5
            ),
        ]);
    }

    private static Level CreateLevel(
        Guid organizationId,
        LevelCode code,
        string name,
        string description,
        LevelTone tone,
        short sortOrder
    )
    {
        return new Level(organizationId, code, name, description, tone, sortOrder);
    }
}
