using Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;

public sealed class EntityType() : LongIdentity, IAggregateRoot
{
    public EntityType(string name, string? description, long microserviceId)
        : this()
    {
        Name = name;
        Description = description;
        MicroserviceId = microserviceId;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public long MicroserviceId { get; private set; }
    public Microservice Microservice { get; private set; } = null!;

    /// <summary>
    /// Обновление сущности
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public EntityType Update(string name, string? description)
    {
        Name = name;
        Description = description;

        return this;
    }
}
