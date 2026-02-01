namespace Edvantix.ProfileService.Features.ProfileFeature.Models;

/// <summary>
/// Модель для обновления истории трудоустройства
/// </summary>
public sealed record UpdateEmploymentHistoryModel
{
    public string Workplace { get; init; } = null!;
    public string Position { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Description { get; init; }
}
