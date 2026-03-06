namespace Edvantix.Catalog.Domain.Events;

/// <summary>
/// Строковые идентификаторы типов сущностей каталога.
/// Используются в <see cref="CatalogEntryChangedEvent"/> для маршрутизации инвалидации кэша.
/// </summary>
public static class CatalogEntityType
{
    public const string Currency = "currency";
    public const string Country = "country";
    public const string Timezone = "timezone";
    public const string Language = "language";
    public const string Region = "region";
}
