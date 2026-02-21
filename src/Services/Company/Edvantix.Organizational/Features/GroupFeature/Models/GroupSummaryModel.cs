namespace Edvantix.Organizational.Features.GroupFeature.Models;

public sealed record GroupSummaryModel(
    ulong Id,
    ulong OrganizationId,
    string Name,
    string? Description,
    string Role
);
