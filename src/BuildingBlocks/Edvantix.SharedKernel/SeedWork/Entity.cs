namespace Edvantix.SharedKernel.SeedWork;

public abstract class Entity : HasDomainEvents
{
    public ulong Id { get; set; }
}

public abstract class Entity<TId> : Entity
    where TId : IEquatable<TId>
{
    public new TId Id { get; set; } = default!;
}
