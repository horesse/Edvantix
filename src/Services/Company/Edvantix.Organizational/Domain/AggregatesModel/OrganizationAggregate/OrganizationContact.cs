using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public sealed class OrganizationContact() : Entity
{
    public OrganizationContact(
        ulong organizationId,
        ContactType type,
        string value,
        string? description = null
    )
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

    public ulong OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public ContactType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public string? Description { get; private set; }
}
