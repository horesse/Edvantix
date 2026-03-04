namespace Edvantix.Catalog.Application.Behaviors;

/// <summary>
/// Маркерный интерфейс для запросов, результат которых должен кэшироваться через <see cref="HybridCache"/>.
/// Запросы, реализующие этот интерфейс, автоматически подхватываются <see cref="CachingBehavior{TMessage,TResponse}"/>.
/// </summary>
public interface ICachedQuery
{
    /// <summary>Уникальный ключ кэша для данного запроса.</summary>
    string CacheKey { get; }

    /// <summary>Теги кэша для групповой инвалидации через <see cref="HybridCache.RemoveByTagAsync"/>.</summary>
    string[] Tags { get; }
}
