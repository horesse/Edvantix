using Edvantix.Groups.Domain.AggregatesModel.LevelAggregate;

namespace Edvantix.Groups.Features.Levels;

/// <summary>Маппер <see cref="Level"/> → <see cref="LevelDto"/>.</summary>
public sealed class LevelDomainToDtoMapper : Mapper<Level, LevelDto>
{
    public override LevelDto Map(Level source) =>
        new(
            source.Id,
            source.Code.Value,
            source.Name,
            source.Description,
            source.Tone,
            source.SortOrder,
            source.IsActive,
            UsageCount: 0 // Placeholder: заполняется когда Group получит FK на Level
        );
}
