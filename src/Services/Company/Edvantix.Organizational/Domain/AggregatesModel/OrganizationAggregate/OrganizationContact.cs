using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public sealed class OrganizationContact() : Entity
{
    public OrganizationContact(
        Guid organizationId,
        ContactType type,
        string value,
        string? description = null
    )
        : this()
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение контакта не может быть пустым.", nameof(value));

        OrganizationId = organizationId;
        Type = type;
        Value = value;
        Description = description;
    }

    public Guid OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ContactType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public string? Description { get; private set; }

    /// <summary>Обновляет тип контакта.</summary>
    public void UpdateType(ContactType type) => Type = type;

    /// <summary>Обновляет значение контакта.</summary>
    public void UpdateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение контакта не может быть пустым.", nameof(value));

        Value = value;
    }

    /// <summary>Обновляет описание контакта.</summary>
    public void UpdateDescription(string? description) => Description = description;
}
