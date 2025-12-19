namespace Edvantix.Chassis.Converter;

public interface IConverter<TFirst, TSecond>
    where TFirst : class
    where TSecond : notnull
{
    TSecond Map(TFirst source);

    TFirst Map(TSecond source);

    IReadOnlyList<TSecond> Map(IReadOnlyList<TFirst> sources);

    IReadOnlyList<TFirst> Map(IReadOnlyList<TSecond> sources);

    void SetProperties(TFirst source, TSecond target);
}
