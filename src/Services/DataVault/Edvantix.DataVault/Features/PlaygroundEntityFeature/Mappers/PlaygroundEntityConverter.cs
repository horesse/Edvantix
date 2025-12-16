using Edvantix.Chassis.Converter;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;
using Edvantix.DataVault.Features.PlaygroundEntityFeature.Models;

namespace Edvantix.DataVault.Features.PlaygroundEntityFeature.Mappers;

public sealed class PlaygroundEntityConverter
    : ClassConverter<PlaygroundEntityModel, PlaygroundEntity>
{
    public override PlaygroundEntity Map(PlaygroundEntityModel source)
    {
        return new PlaygroundEntity(source.Value, source.Name);
    }

    public override PlaygroundEntityModel Map(PlaygroundEntity source)
    {
        return new PlaygroundEntityModel
        {
            Name = source.Name,
            Id = source.Id,
            Value = source.Value,
        };
    }

    public override void SetProperties(PlaygroundEntityModel source, PlaygroundEntity target)
    {
        target.Update(source.Value, source.Name);
    }
}
