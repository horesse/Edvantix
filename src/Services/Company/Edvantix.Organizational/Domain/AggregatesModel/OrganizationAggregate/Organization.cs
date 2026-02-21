using Edvantix.Constants.Other;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public sealed class Organization() : Entity, IAggregateRoot, ISoftDelete
{
    private readonly List<OrganizationContact> _contacts = [];
    private readonly List<OrganizationMember> _members = [];
    private readonly List<Group> _groups = [];

    public Organization(
        string name,
        string nameLatin,
        string shortName,
        DateTime registrationDate,
        string? printName = null,
        string? description = null
    )
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название организации не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(nameLatin))
            throw new ArgumentException(
                "Латинское название не может быть пустым.",
                nameof(nameLatin)
            );

        if (string.IsNullOrWhiteSpace(shortName))
            throw new ArgumentException(
                "Краткое название не может быть пустым.",
                nameof(shortName)
            );

        Name = name;
        NameLatin = nameLatin;
        ShortName = shortName;
        RegistrationDate = registrationDate;
        PrintName = printName;
        Description = description;
    }

    public string Name { get; private set; } = null!;
    public string NameLatin { get; private set; } = null!;
    public string ShortName { get; private set; } = null!;
    public string? PrintName { get; private set; }
    public string? Description { get; private set; }
    public DateTime RegistrationDate { get; private set; }

    public IReadOnlyCollection<OrganizationContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<OrganizationMember> Members => _members.AsReadOnly();
    public IReadOnlyCollection<Group> Groups => _groups.AsReadOnly();

    /// <summary>
    /// Обновляет наименования организации.
    /// </summary>
    public void UpdateNames(
        string name,
        string nameLatin,
        string shortName,
        string? printName = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название организации не может быть пустым.", nameof(name));

        if (string.IsNullOrWhiteSpace(nameLatin))
            throw new ArgumentException(
                "Латинское название не может быть пустым.",
                nameof(nameLatin)
            );

        if (string.IsNullOrWhiteSpace(shortName))
            throw new ArgumentException(
                "Краткое название не может быть пустым.",
                nameof(shortName)
            );

        Name = name;
        NameLatin = nameLatin;
        ShortName = shortName;
        PrintName = printName;
    }

    /// <summary>
    /// Обновляет описание организации.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Добавляет контакт организации. Возвращает созданный объект контакта.
    /// Id будет заполнен EF Core после сохранения.
    /// </summary>
    public OrganizationContact AddContact(
        ContactType type,
        string value,
        string? description = null
    )
    {
        var contact = new OrganizationContact(Id, type, value, description);
        _contacts.Add(contact);
        return contact;
    }

    /// <summary>
    /// Удаляет контакт организации по идентификатору.
    /// </summary>
    public void RemoveContact(ulong contactId)
    {
        var contact =
            _contacts.FirstOrDefault(c => c.Id == contactId)
            ?? throw new InvalidOperationException($"Контакт с ID {contactId} не найден.");

        _contacts.Remove(contact);
    }

    /// <summary>
    /// Обновляет существующий контакт организации.
    /// </summary>
    public void UpdateContact(ulong contactId, ContactType type, string value, string? description)
    {
        var contact =
            _contacts.FirstOrDefault(c => c.Id == contactId)
            ?? throw new InvalidOperationException($"Контакт с ID {contactId} не найден.");

        contact.UpdateType(type);
        contact.UpdateValue(value);
        contact.UpdateDescription(description);
    }

    public bool IsDeleted { get; set; }

    public void Delete()
    {
        IsDeleted = true;
    }
}
