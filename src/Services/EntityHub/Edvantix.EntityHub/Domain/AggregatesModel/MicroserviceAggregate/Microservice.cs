using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;

public sealed class Microservice() : LongIdentity, IAggregateRoot
{
    public Microservice(string name) : this()
    {
        Name = name;
    }

    public string Name { get; init; } = null!;
}
