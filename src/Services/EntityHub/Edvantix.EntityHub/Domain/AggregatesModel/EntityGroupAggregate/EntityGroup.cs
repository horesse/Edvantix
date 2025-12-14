using Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.EntityHub.Domain.AggregatesModel.EntityGroupAggregate;

public sealed class EntityGroup() : LongIdentity, IAggregateRoot
{
    public EntityGroup(string name)
        : this()
    {
        Name = name;
    }

    public string Name { get; set; } = null!;

    public ICollection<EntityType> Entities { get; set; } = null!;
}
