namespace Edvantix.Chassis.Mapper;

public interface IMapper<in TSource, out TDestination>
    where TSource : class
    where TDestination : notnull
{
    /// <summary>
    /// Преобразует исходную модель в целевой объект.
    /// </summary>
    /// <param name="source">Исходная модель для преобразования.</param>
    /// <returns>Преобразованный целевой объект.</returns>
    TDestination Map(TSource source);

    /// <summary>
    /// Преобразует коллекцию исходных моделей в коллекцию целевых объектов.
    /// </summary>
    /// <param name="sources">Коллекция исходных моделей для преобразования.</param>
    /// <returns>Доступная только для чтения коллекция преобразованных целевых объектов.</returns>
    IReadOnlyCollection<TDestination> Map(IReadOnlyCollection<TSource> sources);
}
