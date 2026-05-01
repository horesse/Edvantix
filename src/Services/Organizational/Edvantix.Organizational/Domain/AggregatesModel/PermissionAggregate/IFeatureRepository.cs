namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Репозиторий агрегата <see cref="Feature"/>.
/// Разрешения загружаются автоматически через AutoInclude — отдельный метод не нужен.
/// </summary>
public interface IFeatureRepository : IRepository<Feature>
{
    /// <summary>Возвращает область по коду или <c>null</c>, если не найдена.</summary>
    Task<Feature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новую область в контекст.</summary>
    void Add(Feature feature);
}
