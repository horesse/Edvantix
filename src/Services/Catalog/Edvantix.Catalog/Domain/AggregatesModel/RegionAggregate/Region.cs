namespace Edvantix.Catalog.Domain.AggregatesModel.RegionAggregate;

/// <summary>
/// Географический регион. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Region() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую запись региона.
    /// </summary>
    /// <param name="code">Код региона (например, "NA" для Северной Америки).</param>
    /// <param name="name">Наименование региона (например, "North America").</param>
    public Region(string code, string name)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name;
        IsActive = true;
    }

    /// <summary>Код региона (например, "NA", "EU", "APAC").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Наименование региона (например, "North America", "Europe").</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Признак активности региона в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля региона.
    /// </summary>
    /// <param name="name">Новое наименование.</param>
    /// <param name="isActive">Новый статус активности.</param>
    public void Update(string name, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        IsActive = isActive;
    }
}
