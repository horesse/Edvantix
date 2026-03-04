namespace Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;

/// <summary>
/// Репозиторий для чтения справочника часовых поясов.
/// </summary>
public interface ITimezoneRepository : IRepository<Timezone>
{
    /// <summary>Возвращает список часовых поясов, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Timezone>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );
}
