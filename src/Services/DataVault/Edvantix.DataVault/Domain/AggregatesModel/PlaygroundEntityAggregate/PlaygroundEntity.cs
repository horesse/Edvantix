using Edvantix.Chassis.EF.Attributes;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;

public sealed class PlaygroundEntity() : LongIdentity, IAggregateRoot, ISoftDelete
{
    public PlaygroundEntity(decimal value, string name)
        : this()
    {
        Value = value;
        Name = name;
    }

    [OrderBy(OrderType.ThenByDescending)]
    public decimal Value { get; private set; }
    
    [OrderBy]
    public string Name { get; private set; } = null!;
    public bool IsDeleted { get; set; }

    public void Delete()
    {
        IsDeleted = true;
    }

    public void Update(decimal value, string name)
    {
        Value = value;
        Name = name;
    }
}
