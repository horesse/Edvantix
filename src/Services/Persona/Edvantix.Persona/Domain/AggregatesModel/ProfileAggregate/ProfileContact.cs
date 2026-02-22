using Edvantix.Constants.Other;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class ProfileContact() : PersonalData
{
    internal ProfileContact(ContactType type, string value, string? description = null)
        : this()
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение контакта не может быть пустым.", nameof(value));

        Type = type;
        Value = value;
        Description = description;
    }

    public ContactType Type { get; private set; }
    public string Value { get; private set; } = null!;
    public string? Description { get; private set; }
}
