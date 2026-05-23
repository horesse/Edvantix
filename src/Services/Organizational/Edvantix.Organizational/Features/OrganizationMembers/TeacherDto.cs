namespace Edvantix.Organizational.Features.OrganizationMembers;

/// <summary>Краткое представление участника организации для выбора преподавателя.</summary>
public sealed record TeacherDto(
    Guid MemberId,
    string FullName,
    string PrimaryRole,
    string? AvatarUrl
);
