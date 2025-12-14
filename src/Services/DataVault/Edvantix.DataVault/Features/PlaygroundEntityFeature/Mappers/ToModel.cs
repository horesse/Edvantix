using Edvantix.Chassis.Mapper;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;
using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Mappers;

public sealed class ToModel : IMapper<PlaygroundEntity, PlaygroundEntityModel>
{
    public PlaygroundEntityModel Map(PlaygroundEntity source)
    {
        return new PlaygroundEntityModel
        {
            Name = source.Name,
            Id = source.Id,
            Value = source.Value,
        };
    }

    public IReadOnlyList<PlaygroundEntityModel> Map(IReadOnlyList<PlaygroundEntity> sources)
    {
        return [.. sources.Select(Map)];
    }

    public void SetProperties(PlaygroundEntity source, PlaygroundEntityModel target) =>
        throw new NotImplementedException();
}
