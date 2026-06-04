using Edvantix.Chassis.Mapper;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>Маппер <see cref="StudentStatus"/> → <see cref="StudentStatusDto"/>.</summary>
public sealed class StudentStatusDtoMapper : Mapper<StudentStatus, StudentStatusDto>
{
    /// <inheritdoc/>
    public override StudentStatusDto Map(StudentStatus source) =>
        new(
            source.Id,
            source.Name,
            source.Code,
            source.Tone,
            source.IsSystem,
            source.IsDeleted,
            source.Order,
            source.OrganizationId,
            source.CreatedAt,
            source.LastModifiedAt,
            source.CreatedBy,
            source.LastModifiedBy
        );
}

/// <summary>Маппер <see cref="StudentStatus"/> → <see cref="StudentStatusListItemDto"/>.</summary>
public sealed class StudentStatusListItemDtoMapper : Mapper<StudentStatus, StudentStatusListItemDto>
{
    /// <inheritdoc/>
    public override StudentStatusListItemDto Map(StudentStatus source) =>
        new(
            source.Id,
            source.Name,
            source.Code,
            source.Tone,
            source.IsSystem,
            source.IsDeleted,
            source.Order
        );
}
