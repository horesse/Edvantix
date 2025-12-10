using Edvantix.Chassis.Mapper;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;
using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Mappers;

public sealed class ToEntity : IMapper<PlaygroundEntityModel, PlaygroundEntity>
{
    public PlaygroundEntity Map(PlaygroundEntityModel source)
    {
        return new PlaygroundEntity
        {
            Name = source.Name,
            Id = source.Id,
            Value = source.Value,
        };
    }

    public IReadOnlyList<PlaygroundEntity> Map(IReadOnlyList<PlaygroundEntityModel> sources)
    {
        return [.. sources.Select(Map)];
    }
}
