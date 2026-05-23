using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

namespace Edvantix.Organizational.Features.Directories.LeadSources;

/// <summary>Маппер <see cref="LeadSource"/> → <see cref="LeadSourceDto"/>.</summary>
public sealed class LeadSourceDtoMapper : Mapper<LeadSource, LeadSourceDto>
{
    /// <inheritdoc/>
    public override LeadSourceDto Map(LeadSource source) =>
        new(
            source.Id,
            source.Name,
            source.Channel,
            source.UtmTag,
            source.IsArchived,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy
        );
}

/// <summary>Маппер <see cref="LeadSource"/> → <see cref="LeadSourceListItemDto"/>.</summary>
public sealed class LeadSourceListItemDtoMapper : Mapper<LeadSource, LeadSourceListItemDto>
{
    /// <inheritdoc/>
    public override LeadSourceListItemDto Map(LeadSource source) =>
        new(source.Id, source.Name, source.Channel, source.UtmTag, source.IsArchived, source.Order);
}
