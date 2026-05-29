using Edvantix.Organizational.Domain.AggregatesModel.SubjectAggregate;

namespace Edvantix.Organizational.Features.Directories.Subjects;

/// <summary>Маппер <see cref="Subject"/> → <see cref="SubjectDto"/>.</summary>
public sealed class SubjectDomainToDtoMapper : Mapper<Subject, SubjectDto>
{
    public override SubjectDto Map(Subject source) =>
        new(
            source.Id,
            source.Name,
            source.Code.Value,
            source.Color,
            source.Description,
            source.Order,
            source.IsDeleted,
            source.CreatedAt,
            source.LastModifiedAt
        );
}

/// <summary>Маппер <see cref="Subject"/> → <see cref="SubjectListItemDto"/>.</summary>
public sealed class SubjectDomainToListItemDtoMapper : Mapper<Subject, SubjectListItemDto>
{
    public override SubjectListItemDto Map(Subject source) =>
        new(
            source.Id,
            source.Name,
            source.Code.Value,
            source.Color,
            source.Order,
            source.IsDeleted
        );
}
