namespace Edvantix.Organizational.Features.OrganizationFeature.Models;

public sealed record OrganizationSummaryModel(
    Guid Id,
    string Name,
    string ShortName,
    string? Description,
    string Role
);
