namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>Репозиторий агрегата <see cref="Feature"/>.</summary>
public interface IFeatureRepository : IRepository<Feature>
{
    /// <summary>Возвращает все функциональные области.</summary>
    Task<List<Feature>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Возвращает функциональную область по уникальному коду.</summary>
    Task<Feature?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>Добавляет функциональную область в контекст.</summary>
    void Add(Feature feature);
}
