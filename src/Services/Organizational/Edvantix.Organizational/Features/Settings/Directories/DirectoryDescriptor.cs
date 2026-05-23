namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Метаданные одного справочника в каталоге настроек.
/// </summary>
/// <param name="Code">Стабильный машинный код (kebab-case), напр. <c>levels</c>, <c>lesson-types</c>.</param>
/// <param name="Name">Локализованное отображаемое имя.</param>
/// <param name="Description">Локализованное краткое описание для карточки в UI.</param>
/// <param name="Icon">Имя иконки Lucide (как в frontend-каталоге).</param>
/// <param name="Badge">Опциональный системный бейдж (напр. <c>системный</c>).</param>
public sealed record DirectoryDescriptor(
    string Code,
    string Name,
    string Description,
    string Icon,
    string? Badge
);
