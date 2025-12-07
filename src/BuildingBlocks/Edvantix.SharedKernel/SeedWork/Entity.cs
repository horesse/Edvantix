namespace Edvantix.SharedKernel.SeedWork;

public abstract class Entity : HasDomainEvents
{
    public long Id { get; set; }
}

public abstract class Entity<TId> : Entity
    where TId : struct
{
    public new TId Id { get; set; } = default!;
}
