namespace Edvantix.Organizational.Features.GroupFeature.Models;

public sealed record GroupSummaryModel(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Description,
    string Role
);
