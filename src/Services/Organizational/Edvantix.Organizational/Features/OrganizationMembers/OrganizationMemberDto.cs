using Edvantix.Organizational.Domain.Enums;

namespace Edvantix.Organizational.Features.OrganizationMembers;

public sealed record OrganizationMemberDto(
    Guid Id,
    Guid OrganizationId,
    Guid ProfileId,
    Guid OrganizationMemberRoleId,
    OrganizationStatus Status,
    DateOnly StartDate,
    DateOnly? EndDate
);
