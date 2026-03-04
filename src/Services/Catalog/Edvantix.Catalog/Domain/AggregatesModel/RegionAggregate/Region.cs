namespace Edvantix.Catalog.Domain.AggregatesModel.RegionAggregate;

/// <summary>
/// Географический макрорегион для сегментации рынков. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Region : HasDomainEvents, IAggregateRoot
{
    /// <summary>EF Core требует параметризованный или безпараметровый конструктор для материализации.</summary>
    private Region() { }

    /// <summary>
    /// Создаёт новую запись региона и публикует событие <see cref="CatalogChangeType.Created"/>.
    /// </summary>
    /// <param name="code">Код региона (например, "CIS", "EU", "APAC"). Натуральный PK.</param>
    /// <param name="nameRu">Наименование региона на русском (например, "СНГ").</param>
    /// <param name="nameEn">Наименование региона на английском (например, "CIS").</param>
    public Region(string code, string nameRu, string nameEn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);

        Code = code.ToUpperInvariant();
        NameRu = nameRu;
        NameEn = nameEn;
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(CatalogEntityType.Region, Code, CatalogChangeType.Created)
        );
    }

    /// <summary>Код региона, натуральный PK (например, "CIS", "EU", "APAC", "MENA").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование региона на русском (например, "СНГ", "Европа").</summary>
    public string NameRu { get; private set; } = null!;

    /// <summary>Наименование региона на английском (например, "CIS", "Europe").</summary>
    public string NameEn { get; private set; } = null!;

    /// <summary>Признак активности региона в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля региона и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    /// <param name="nameRu">Новое наименование на русском.</param>
    /// <param name="nameEn">Новое наименование на английском.</param>
    public void Update(string nameRu, string nameEn)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nameRu);
        ArgumentException.ThrowIfNullOrWhiteSpace(nameEn);

        NameRu = nameRu;
        NameEn = nameEn;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(CatalogEntityType.Region, Code, CatalogChangeType.Updated)
        );
    }

    /// <summary>
    /// Активирует регион и публикует событие <see cref="CatalogChangeType.Updated"/>.
    /// </summary>
    public void Activate()
    {
        IsActive = true;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(CatalogEntityType.Region, Code, CatalogChangeType.Updated)
        );
    }

    /// <summary>
    /// Деактивирует регион и публикует событие <see cref="CatalogChangeType.Deactivated"/>.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;

        RegisterDomainEvent(
            new CatalogEntryChangedEvent(
                CatalogEntityType.Region,
                Code,
                CatalogChangeType.Deactivated
            )
        );
    }
}
