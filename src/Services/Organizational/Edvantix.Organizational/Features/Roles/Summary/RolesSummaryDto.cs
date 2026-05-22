namespace Edvantix.Organizational.Features.Roles.Summary;

/// <summary>
/// Сводная информация о ролях организации для блока «Доступы».
/// </summary>
/// <param name="TotalRoles">Общее количество ролей в организации.</param>
/// <param name="AssignedMembersCount">Количество участников с назначенной ролью.</param>
/// <param name="RoleNamesPreview">
/// Названия первых пяти ролей, отсортированных по дате создания.
/// </param>
public sealed record RolesSummaryDto(
    int TotalRoles,
    int AssignedMembersCount,
    IReadOnlyList<string> RoleNamesPreview
);
