namespace Edvantix.Catalog.Domain.AggregatesModel.CountryAggregate;

/// <summary>
/// Репозиторий для чтения справочника стран.
/// </summary>
public interface ICountryRepository : IRepository<Country>
{
    /// <summary>Возвращает список стран, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Country>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает страну по ISO 3166-1 alpha-2 коду или <c>null</c>, если не найдена.</summary>
    /// <param name="code">Двухбуквенный код ISO 3166-1 alpha-2 (например, "US").</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
