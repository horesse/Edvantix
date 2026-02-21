namespace Edvantix.Organizational.Features.GroupFeature.Models;

public sealed record GroupSummaryModel(
    long Id,
    long OrganizationId,
    string Name,
    string? Description,
    string Role
);
