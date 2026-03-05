namespace Edvantix.Catalog.Application.EventHandlers;

/// <summary>
/// Обрабатывает <see cref="CatalogEntryChangedEvent"/> — инвалидирует соответствующий тег HybridCache.
/// Один вызов <see cref="HybridCache.RemoveByTagAsync"/> сбрасывает все записи (list и by-code)
/// для изменённого типа сущности, не требуя перечисления конкретных ключей.
/// </summary>
public sealed class CatalogEntryChangedEventHandler(
    HybridCache cache,
    ILogger<CatalogEntryChangedEventHandler> logger
) : INotificationHandler<CatalogEntryChangedEvent>
{
    public async ValueTask Handle(
        CatalogEntryChangedEvent notification,
        CancellationToken cancellationToken
    )
    {
        // Инвалидируем тег, соответствующий типу изменённой сущности
        var tag = notification.EntityType;

        logger.LogInformation(
            "Catalog entry changed: {EntityType}/{Code} ({ChangeType}). Invalidating cache tag '{Tag}'.",
            notification.EntityType,
            notification.Code,
            notification.ChangeType,
            tag
        );

        await cache.RemoveByTagAsync(tag, cancellationToken);
    }
}
