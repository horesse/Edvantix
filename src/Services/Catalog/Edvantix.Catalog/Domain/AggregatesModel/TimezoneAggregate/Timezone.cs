namespace Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;

/// <summary>
/// Часовой пояс по стандарту IANA. Справочная сущность, управляемая администраторами платформы.
/// </summary>
public sealed class Timezone() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт новую запись часового пояса.
    /// </summary>
    /// <param name="code">Идентификатор IANA (например, "America/New_York").</param>
    /// <param name="name">Отображаемое название (например, "Eastern Time").</param>
    /// <param name="utcOffsetMinutes">Смещение UTC в минутах (например, -300 для UTC-5).</param>
    public Timezone(string code, string name, int utcOffsetMinutes)
        : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name;
        UtcOffsetMinutes = utcOffsetMinutes;
        IsActive = true;
    }

    /// <summary>Идентификатор часового пояса IANA (например, "America/New_York", "Europe/Berlin").</summary>
    public string Code { get; private set; } = null!;

    /// <summary>Отображаемое название часового пояса (например, "Eastern Time").</summary>
    public string Name { get; private set; } = null!;

    /// <summary>Смещение относительно UTC в минутах (например, -300 для UTC-5).</summary>
    public int UtcOffsetMinutes { get; private set; }

    /// <summary>Признак активности часового пояса в системе.</summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Обновляет изменяемые поля часового пояса.
    /// </summary>
    /// <param name="name">Новое отображаемое название.</param>
    /// <param name="utcOffsetMinutes">Новое смещение UTC в минутах.</param>
    /// <param name="isActive">Новый статус активности.</param>
    public void Update(string name, int utcOffsetMinutes, bool isActive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        UtcOffsetMinutes = utcOffsetMinutes;
        IsActive = isActive;
    }
}
