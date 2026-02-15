using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Company.Features.GroupFeature.Models;

public sealed record GroupSummaryModel(
    long Id,
    long OrganizationId,
    string Name,
    string? Description,
    string Role
);
