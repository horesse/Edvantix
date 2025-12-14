namespace Edvantix.SharedKernel.SeedWork;

public abstract class Entity<TId> : HasDomainEvents
    where TId : struct
{
    public TId Id { get; set; } = default!;
}

public abstract class LongIdentity : Entity<long>;
