using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organization.Domain.AggregatesModel.ContactAggregate;

public sealed class Contact() : LongIdentity, IAggregateRoot
{
    public Contact(long organizationId, ContactType type, string value, string? description = null)
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение контакта не может быть пустым.", nameof(value));

        OrganizationId = organizationId;
        Type = type;
        Value = value;
        Description = description;
    }

    public long OrganizationId { get; private set; }
    public OrganizationAggregate.Organization Organization { get; private set; } = null!;
    public ContactType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public string? Description { get; private set; }

    public void UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException(
                "Значение контакта не может быть пустым.",
                nameof(newValue)
            );

        Value = newValue;
    }

    public void UpdateType(ContactType newType)
    {
        Type = newType;
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }
}
