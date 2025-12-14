using System.Diagnostics.CodeAnalysis;

namespace Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;

[ExcludeFromCodeCoverage]
public sealed class PlaygroundEntityData : List<PlaygroundEntity>
{
    public PlaygroundEntityData()
    {
        Add(new PlaygroundEntity(1, "Demo"));
    }
}
