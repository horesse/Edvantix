namespace Edvantix.Organizational.Features.OrganizationMembers;

/// <summary>
/// KPI-статистика участников организации: количество по каждому статусу.
/// </summary>
public sealed record OrganizationMembersKpiDto(
    [property: Description("Общее количество участников")] int Total,
    [property: Description("Количество активных участников")] int Active,
    [property: Description("Количество архивированных участников")] int Archived,
    [property: Description("Количество удалённых участников")] int Deleted
);
