namespace Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;

/// <summary>
/// Язык по стандарту ISO 639-1. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Language : HasDomainEvents, IAggregateRoot
{
    /// <summary>EF Core требует параметризованный или безпараметровый конструктор для материализации.</summary>
    private Language() { }

    /// <summary>
    /// Создаёт новую запись языка и публикует событие <see cref="CatalogChangeType.Created"/>.
    /// </summary>
    /// <param name="code">Двухбуквенный код ISO 639-1 (например, "en", "ru"). Натуральный PK.</param>
    /// <param name="nameRu">Наименование языка на русском (например, "Английский").</param>
    /// <param name="nameEn">Наименование языка на английском (например, "English").</param>
    /// <param name="nativeName">Название языка на родном языке (например, "English", "Русский").</param>
    public Language(string code, string nameRu, string nameEn, string nativeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(nativeName);

        Code = code.ToLowerInvariant();
        NameRu = nameRu;
        NameEn = nameEn;
        NativeName = nativeName;
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Language,
                Code,
                CatalogChangeType.Created
            )
        );
    }

    /// <summary>Двухбуквенный код ISO 639-1, натуральный PK (например, "en", "ru", "de").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование языка на русском (например, "Английский", "Немецкий").</summary>
    public string NameRu { get; private set; } = null!;

    /// <summary>Наименование языка на английском (например, "English", "German").</summary>
    public string NameEn { get; private set; } = null!;

    /// <summary>Название языка на родном языке (например, "Deutsch", "Русский").</summary>
    public string NativeName { get; private set; } = null!;

    /// <summary>Признак активности языка в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля языка и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    /// <param name="nameRu">Новое наименование на русском.</param>
    /// <param name="nameEn">Новое наименование на английском.</param>
    /// <param name="nativeName">Новое название на родном языке.</param>
    public void Update(string nameRu, string nameEn, string nativeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);
        ArgumentException.ThrowIfNullOrWhiteSpace(nativeName);

        NameRu = nameRu;
        NameEn = nameEn;
        NativeName = nativeName;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Language,
                Code,
                CatalogChangeType.Updated
            )
        );
    }

    /// <summary>
    /// Активирует язык и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    public void Activate()
    {
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Language,
                Code,
                CatalogChangeType.Updated
            )
        );
    }

    /// <summary>
    /// Деактивирует язык и публикует событие <see cref="CatalogChangeType.Deactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Language,
                Code,
                CatalogChangeType.Deactivated
            )
        );
    }
}
