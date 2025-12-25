using Edvantix.Constants.Other;
using Edvantix.Person.Domain.Abstractions;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.AggregatesModel.ContactAggregate;

public sealed class Contact : PersonalData<long>, ISoftDelete, IAggregateRoot
{
    private Contact() { }

    internal Contact(ContactType type, string value, string? description = null)
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

    public bool IsDeleted { get; set; }

    internal void UpdateValue(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
            throw new ArgumentException(
                "Значение контакта не может быть пустым.",
                nameof(newValue)
            );

        Value = newValue;
    }

    internal void UpdateType(ContactType newType)
    {
        Type = newType;
    }

    internal void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
