namespace Edvantix.Organizational.Features.LegalFormFeature.Models;

/// <summary>
/// Модель организационно-правовой формы для ответов API.
/// </summary>
public sealed class LegalFormModel
{
    /// <summary>Идентификатор записи.</summary>
    public Guid Id { get; set; }

    /// <summary>Полное наименование.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Сокращённое наименование (аббревиатура).</summary>
    public string ShortName { get; set; } = null!;
}
