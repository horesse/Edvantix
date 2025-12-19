namespace Edvantix.Chassis.Converter;

public abstract class ClassConverter<TFirst, TSecond> : IConverter<TFirst, TSecond>
    where TFirst : class
    where TSecond : notnull
{
    public abstract TSecond Map(TFirst source);

    public abstract TFirst Map(TSecond source);

    public virtual IReadOnlyList<TSecond> Map(IReadOnlyList<TFirst> sources)
    {
        return [.. sources.Select(Map)];
    }

    public virtual IReadOnlyList<TFirst> Map(IReadOnlyList<TSecond> sources)
    {
        return [.. sources.Select(Map)];
    }

    public abstract void SetProperties(TFirst source, TSecond target);
}
