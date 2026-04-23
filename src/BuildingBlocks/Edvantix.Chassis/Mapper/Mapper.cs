namespace Edvantix.Chassis.Mapper;

public abstract class Mapper<TFirst, TSecond> : IMapper<TFirst, TSecond>
    where TSecond : notnull
    where TFirst : class
{
    public abstract TSecond Map(TFirst source);

    public virtual IReadOnlyCollection<TSecond> Map(IReadOnlyCollection<TFirst> sources)
    {
        return [.. sources.Select(Map)];
    }
}
