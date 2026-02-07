namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

/// <summary>
/// Модель для обновления образования
/// </summary>
public sealed record UpdateEducationModel
{
    public DateTime DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public string Institution { get; init; } = null!;
    public string? Specialty { get; init; }
    public long EducationLevelId { get; init; }
}
