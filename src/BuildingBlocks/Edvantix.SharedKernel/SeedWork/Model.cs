namespace Edvantix.SharedKernel.SeedWork;

public abstract class Model
{
    public long Id { get; set; }
}

public abstract class Model<TId> : Entity
    where TId : struct
{
    public new TId Id { get; set; } = default!;
}
