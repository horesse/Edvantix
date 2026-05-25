namespace Edvantix.Organizational.Features.Directories.Levels;

/// <summary>Строка списка справочника «Уровни».</summary>
/// <param name="Id">Идентификатор уровня.</param>
/// <param name="Name">Отображаемое название.</param>
/// <param name="Order">Порядковый номер в списках.</param>
/// <param name="Description">Описание уровня.</param>
/// <param name="IsArchived">Деактивирован ли уровень.</param>
public sealed record LevelDirectoryListItemDto(
    Guid Id,
    string Name,
    short Order,
    string? Description,
    bool IsArchived
);
