using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Levels;

/// <summary>DTO уровня для передачи данных клиенту.</summary>
/// <param name="Id">Идентификатор уровня.</param>
/// <param name="Code">Уникальный код уровня (напр. <c>A1</c>).</param>
/// <param name="Name">Отображаемое название уровня.</param>
/// <param name="Description">Описание уровня.</param>
/// <param name="Tone">Цветовой тон для UI-бейджа.</param>
/// <param name="SortOrder">Порядковый номер в выпадающих списках.</param>
/// <param name="IsActive">Доступен ли уровень для выбора в новых группах.</param>
/// <param name="UsageCount">Количество групп, использующих этот уровень.</param>
public sealed record LevelDto(
    Guid Id,
    string Code,
    string Name,
    string? Description,
    LevelTone Tone,
    short SortOrder,
    bool IsActive,
    int UsageCount
);
