namespace Edvantix.Organizational.Features.Directories;

/// <summary>Данные об использовании записи справочника другими объектами.</summary>
/// <param name="Label">Название метрики (напр. «Группы»).</param>
/// <param name="Count">Количество объектов, ссылающихся на запись справочника.</param>
public sealed record DirectoryUsageDto(string Label, int Count);
