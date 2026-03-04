namespace Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;

/// <summary>
/// Репозиторий для чтения справочника языков.
/// </summary>
public interface ILanguageRepository : IRepository<Language>
{
    /// <summary>Возвращает список языков, опционально только активных.</summary>
    /// <param name="activeOnly">Если <c>true</c> — только активные записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<Language>> ListAsync(
        bool activeOnly = true,
        CancellationToken cancellationToken = default
    );
}
