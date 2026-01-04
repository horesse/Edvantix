namespace Edvantix.Chassis.Converter;

public interface IConverter<TFirst, TSecond>
    where TFirst : class
    where TSecond : notnull
{
    TSecond Map(TFirst source);

    TFirst Map(TSecond source);

    List<TSecond> Map(IReadOnlyList<TFirst> sources);

    List<TFirst> Map(IReadOnlyList<TSecond> sources);

    void SetProperties(TFirst source, TSecond target);
}
