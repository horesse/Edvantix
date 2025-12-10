using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;

public sealed class PlaygroundEntity() : LongIdentity, IAggregateRoot
{
    public PlaygroundEntity(decimal value, string name)
        : this()
    {
        Value = value;
        Name = name;
    }

    public decimal Value { get; set; }
    public string Name { get; set; } = null!;
}
