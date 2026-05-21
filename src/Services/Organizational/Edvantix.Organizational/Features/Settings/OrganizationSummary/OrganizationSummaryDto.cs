using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Features.Organizations;

namespace Edvantix.Organizational.Features.Settings.OrganizationSummary;

/// <summary>
/// Сводная информация об организации для страницы настроек.
/// </summary>
public sealed record OrganizationSummaryDto(
    Guid Id,
    string FullLegalName,
    string? ShortName,
    OrganizationType OrganizationType,
    OrganizationStatus Status,
    bool IsLegalEntity,
    int MembersCount,
    ContactDto? PrimaryContact,
    OrganizationSummaryDto.LastModifiedInfo LastModified
)
{
    /// <summary>Информация о последнем изменении организации.</summary>
    public sealed record LastModifiedInfo(DateTime? At, string? ByDisplayName);
}
