namespace Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;

/// <summary>
/// Часовой пояс по стандарту IANA TZ Database. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Timezone : HasDomainEvents, IAggregateRoot
{
    /// <summary>EF Core требует параметризованный или безпараметровый конструктор для материализации.</summary>
    private Timezone() { }

    /// <summary>
    /// Создаёт новую запись часового пояса и публикует событие <see cref="CatalogChangeType.Created"/>.
    /// </summary>
    /// <param name="code">Идентификатор IANA TZ Database (например, "Europe/Moscow"). Натуральный PK.</param>
    /// <param name="nameRu">Наименование на русском языке (например, "Московское время").</param>
    /// <param name="nameEn">Наименование на английском языке (например, "Moscow Time").</param>
    /// <param name="displayName">Отображаемое название со смещением (например, "(UTC+03:00) Moscow").</param>
    /// <param name="utcOffsetMinutes">Смещение UTC в минутах (например, 180 для UTC+3).</param>
    public Timezone(
        string code,
        string nameRu,
        string nameEn,
        string displayName,
        int utcOffsetMinutes
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Code = code;
        NameRu = nameRu;
        NameEn = nameEn;
        DisplayName = displayName;
        UtcOffsetMinutes = utcOffsetMinutes;
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Timezone,
                Code,
                CatalogChangeType.Created
            )
        );
    }

    /// <summary>Идентификатор IANA TZ Database, натуральный PK (например, "America/New_York", "Europe/Berlin").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование на русском (например, "Московское время").</summary>
    public string NameRu { get; private set; } = null!;

    /// <summary>Наименование на английском (например, "Moscow Time").</summary>
    public string NameEn { get; private set; } = null!;

    /// <summary>Отображаемое название со смещением UTC (например, "(UTC+03:00) Moscow, St. Petersburg").</summary>
    public string DisplayName { get; private set; } = null!;

    /// <summary>Смещение относительно UTC в минутах (например, 180 для UTC+3, -300 для UTC-5).</summary>
    public int UtcOffsetMinutes { get; private set; }

    /// <summary>Признак активности часового пояса в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля часового пояса и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    /// <param name="nameRu">Новое наименование на русском.</param>
    /// <param name="nameEn">Новое наименование на английском.</param>
    /// <param name="displayName">Новое отображаемое название.</param>
    /// <param name="utcOffsetMinutes">Новое смещение UTC в минутах.</param>
    public void Update(string nameRu, string nameEn, string displayName, int utcOffsetMinutes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        NameRu = nameRu;
        NameEn = nameEn;
        DisplayName = displayName;
        UtcOffsetMinutes = utcOffsetMinutes;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Timezone,
                Code,
                CatalogChangeType.Updated
            )
        );
    }

    /// <summary>
    /// Деактивирует часовой пояс и публикует событие <see cref="CatalogChangeType.Deactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Timezone,
                Code,
                CatalogChangeType.Deactivated
            )
        );
    }
}
