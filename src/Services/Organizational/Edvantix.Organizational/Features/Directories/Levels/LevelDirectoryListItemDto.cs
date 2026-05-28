using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Organizational.Features.Directories.Levels;

/// <summary>Строка списка справочника «Уровни».</summary>
/// <param name="Id">Идентификатор уровня.</param>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Order">Порядковый номер в списках.</param>
/// <param name="Description">Описание уровня.</param>
/// <param name="IsArchived">Деактивирован ли уровень.</param>
/// <param name="Code">Уникальный код уровня (например A1, B2).</param>
/// <param name="Tone">Цветовой тон бейджа в UI.</param>
/// <param name="Usage">Использование уровня в других объектах (напр. группы).</param>
public sealed record LevelDirectoryListItemDto(
    Guid Id,
    string Name,
    short Order,
    string? Description,
    bool IsArchived,
    string Code,
    LevelTone Tone,
    IReadOnlyList<DirectoryUsageDto> Usage
);
