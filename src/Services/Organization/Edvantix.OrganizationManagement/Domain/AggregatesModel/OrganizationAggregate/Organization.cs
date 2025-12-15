using Edvantix.OrganizationManagement.Domain.AggregatesModel.ContactAggregate;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.MemberAggregate;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;

public sealed class Organization() : LongIdentity, IAggregateRoot
{
    private readonly List<Contact> _contacts = new();
    private readonly List<Member> _members = new();
    private readonly List<Subscription> _subscriptions = new();

    public Organization(
        string name,
        string nameLatin,
        string shortName,
        DateTime registrationDate,
        string? printName = null,
        string? description = null)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Название организации не может быть пустым.",
                nameof(name)
            );
        
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

    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();
    public IReadOnlyCollection<Subscription> Subscriptions => _subscriptions.AsReadOnly();

    public void UpdateNames(string name, string nameLatin, string shortName, string? printName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Название организации не может быть пустым.",
                nameof(name)
            );
        
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

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    // Управление контактами
    public void AddContact(ContactType type, string value, string? description = null)
    {
        var contact = new Contact(Id, type, value, description);
        _contacts.Add(contact);
    }

    public void RemoveContact(long contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null)
            throw new InvalidOperationException($"Контакт с ID {contactId} не найден.");

        _contacts.Remove(contact);
    }

    // Управление членами
    public void AddMember(Guid personId, string? position = null)
    {
        if (_members.Any(m => m.PersonId == personId && !m.IsDeleted))
            throw new InvalidOperationException(
                $"Пользователь {personId} уже является членом данной организации."
            );

        var member = new Member(Id, personId, position);
        _members.Add(member);
    }

    public void RemoveMember(Guid memberId)
    {
        var member = _members.FirstOrDefault(m => m.Id == memberId);
        if (member == null)
            throw new InvalidOperationException($"Член организации с ID {memberId} не найден.");

        member.Delete();
    }

    // Управление подписками
    public void AddSubscription(long subscriptionId, DateTime dateStart, DateTime? dateEnd = null)
    {
        var subscription = new Subscription(subscriptionId, Id, dateStart, dateEnd);
        _subscriptions.Add(subscription);
    }

    public Subscription? GetActiveSubscription()
    {
        return _subscriptions.FirstOrDefault(s => s.IsActive());
    }
}

