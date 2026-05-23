using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;

namespace Edvantix.Organizational.Features.Directories.StudentTags;

/// <summary>Маппер агрегата <see cref="StudentTag"/> в <see cref="StudentTagDto"/>.</summary>
public sealed class StudentTagDtoMapper : Mapper<StudentTag, StudentTagDto>
{
    /// <inheritdoc/>
    public override StudentTagDto Map(StudentTag source) =>
        new(
            source.Id,
            source.Name,
            source.Color,
            source.IsArchived,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy
        );
}

/// <summary>Маппер агрегата <see cref="StudentTag"/> в <see cref="StudentTagListItemDto"/>.</summary>
public sealed class StudentTagListItemDtoMapper : Mapper<StudentTag, StudentTagListItemDto>
{
    /// <inheritdoc/>
    public override StudentTagListItemDto Map(StudentTag source) =>
        new(source.Id, source.Name, source.Color, source.IsArchived, source.Order);
}
