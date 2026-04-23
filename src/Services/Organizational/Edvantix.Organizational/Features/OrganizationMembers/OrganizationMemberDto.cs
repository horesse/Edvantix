using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.OrganizationMembers;

public sealed record OrganizationMemberDto(
    Guid Id,
    Guid ProfileId,
    string Role,
    OrganizationStatus Status
)
{
    public string FullName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public DateTime? LastActivity { get; init; }
}
