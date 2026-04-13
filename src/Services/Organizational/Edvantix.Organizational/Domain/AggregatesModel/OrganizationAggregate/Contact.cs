using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Organizational.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Контактные данные, привязанные к организации.
/// Одна запись — один канал связи определённого типа (email, телефон, мессенджер).
/// Бизнес-правило: в организации должен быть ровно один контакт с <see cref="IsPrimary"/> = true.
/// </summary>
public sealed class Contact() : Entity, ISoftDelete, ITenanted
{
    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="value">Значение контакта (адрес email, номер телефона, username мессенджера).</param>
    /// <param name="description">Описание или назначение контакта (рабочий, личный и т.д.).</param>
    /// <param name="contactType">Тип контакта.</param>
    /// <param name="isPrimary">Признак основного контакта организации.</param>
    public Contact(
        Guid organizationId,
        string value,
        string description,
        ContactType contactType,
        bool isPrimary = false
    )
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));

        OrganizationId = organizationId;
        Value = value.Trim();
        Description = description.Trim();
        ContactType = contactType;
        IsPrimary = isPrimary;
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Значение контакта (адрес email, номер телефона, username мессенджера).</summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>Описание или назначение контакта.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Тип контакта.</summary>
    public ContactType ContactType { get; private set; }

    /// <summary>Признак основного контакта организации. Ровно один активный контакт должен быть Primary.</summary>
    public bool IsPrimary { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Назначает данный контакт основным.</summary>
    public void SetPrimary() => IsPrimary = true;

    /// <summary>Снимает признак основного контакта.</summary>
    public void UnsetPrimary() => IsPrimary = false;

    /// <inheritdoc />
    public void Delete() => IsDeleted = true;
}
