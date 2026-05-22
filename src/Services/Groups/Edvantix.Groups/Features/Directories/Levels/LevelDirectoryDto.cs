using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Directories.Levels;

/// <summary>Полная запись справочника «Уровни» для GET /{id}.</summary>
/// <param name="Id">Идентификатор уровня.</param>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Order">Порядковый номер в списках.</param>
/// <param name="Description">Описание уровня.</param>
/// <param name="IsArchived">Деактивирован ли уровень (недоступен для новых групп).</param>
/// <param name="Code">Внутренний уникальный код уровня.</param>
/// <param name="Tone">Цветовой тон бейджа в UI.</param>
public sealed record LevelDirectoryDto(
    Guid Id,
    string Name,
    short Order,
    string? Description,
    bool IsArchived,
    string Code,
    LevelTone Tone
);
