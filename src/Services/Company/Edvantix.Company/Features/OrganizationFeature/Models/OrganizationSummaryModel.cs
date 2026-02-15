namespace Edvantix.Company.Features.OrganizationFeature.Models;

public sealed record OrganizationSummaryModel(
    long Id,
    string Name,
    string ShortName,
    string? Description,
    string Role
);
