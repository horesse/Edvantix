namespace Edvantix.Organizational.Features.OrganizationFeature.Models;

public sealed record OrganizationSummaryModel(
    ulong Id,
    string Name,
    string ShortName,
    string? Description,
    string Role
);
