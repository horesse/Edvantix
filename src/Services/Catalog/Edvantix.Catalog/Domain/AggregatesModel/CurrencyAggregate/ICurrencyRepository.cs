namespace Edvantix.Catalog.Domain.AggregatesModel.CurrencyAggregate;

/// <summary>
/// Репозиторий для чтения справочника валют.
/// </summary>
public interface ICurrencyRepository : IRepository<Currency>
{
    /// <summary>Возвращает список валют, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Currency>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает валюту по ISO 4217-коду или <c>null</c>, если не найдена.</summary>
    /// <param name="code">Алфавитный код ISO 4217 (например, "USD").</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает отслеживаемую EF Core сущность для операций записи или <c>null</c>.
    /// В отличие от <see cref="GetByCodeAsync"/>, не использует <c>AsNoTracking</c>.
    /// </summary>
    Task<Currency?> FindTrackedByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новую валюту в контекст EF Core.</summary>
    Task AddAsync(Currency entity, CancellationToken cancellationToken = default);
}
