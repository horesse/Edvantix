namespace Edvantix.Catalog.Domain.Events;

/// <summary>
/// Доменное событие изменения справочной записи.
/// Публикуется командами после успешной записи в БД.
/// Обрабатывается handler'ом инвалидации HybridCache (EDV-77).
/// </summary>
/// <param name="EntityType">Тип сущности — одна из констант <see cref="CatalogEntityType"/>.</param>
/// <param name="Code">Код изменённой записи (натуральный ключ).</param>
/// <param name="ChangeType">Характер изменения.</param>
public sealed class CatalogEntryChangedEvent(
    string EntityType,
    string Code,
    CatalogChangeType ChangeType
) : DomainEvent
{
    /// <summary>Тип сущности каталога (<see cref="CatalogEntityType"/>).</summary>
    public string EntityType { get; } = EntityType;

    /// <summary>Код изменённой записи (натуральный PK).</summary>
    public string Code { get; } = Code;

    /// <summary>Характер изменения.</summary>
    public CatalogChangeType ChangeType { get; } = ChangeType;
}
