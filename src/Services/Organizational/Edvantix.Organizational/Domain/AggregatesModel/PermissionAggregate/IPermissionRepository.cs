namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Репозиторий агрегата <see cref="Permission"/>.
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    /// <summary>Возвращает все разрешения из базы данных.</summary>
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает все разрешения с загруженной навигацией <see cref="Permission.Feature"/>.
    /// Используется там, где нужно отображать разрешен��я сгруппированными по функциональной области.
    /// </summary>
    Task<List<Permission>> GetAllWithFeaturesAsync(CancellationToken cancellationToken = default);

    /// <summary>Возвращает общее количество разрешений.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Добавляет разрешение в контекст.</summary>
    void Add(Permission permission);

    /// <summary>Помечает разрешение для удаления.</summary>
    void Remove(Permission permission);
}
