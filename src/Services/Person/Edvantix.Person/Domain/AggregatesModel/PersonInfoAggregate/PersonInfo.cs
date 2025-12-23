using Edvantix.Chassis.EF.Attributes;
using Edvantix.Constants.Other;
using Edvantix.Person.Domain.AggregatesModel.ContactAggregate;
using Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;

public sealed class PersonInfo() : Entity<long>, IAggregateRoot, ISoftDelete
{
    private readonly List<Contact> _contacts = new();
    private readonly List<EmploymentHistory> _employmentHistories = new();

    public PersonInfo(
        Guid accountId,
        Gender gender,
        string firstName,
        string lastName,
        string? middleName = null
    )
        : this()
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId не может быть пустым.", nameof(accountId));

        AccountId = accountId;
        Gender = gender;
        FullName = new FullName(firstName, lastName, middleName);
    }

    public Guid AccountId { get; private set; }
    public Gender Gender { get; private set; }

    [Include]
    public FullName FullName { get; private set; } = null!;

    public IReadOnlyCollection<Contact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();

    public bool IsDeleted { get; set; }

    // Методы для управления Gender
    public void UpdateGender(Gender newGender)
    {
        Gender = newGender;
    }

    // Методы для управления FullName
    public void UpdateFullName(string firstName, string lastName, string? middleName = null)
    {
        FullName.Update(firstName, lastName, middleName);
    }

    // Методы для управления Contacts
    public Contact AddContact(ContactType type, string value, string? description = null)
    {
        var contact = new Contact(type, value, description);
        _contacts.Add(contact);
        return contact;
    }

    public void RemoveContact(long contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null)
            throw new InvalidOperationException($"Контакт с ID {contactId} не найден.");

        contact.Delete();
    }

    public void UpdateContact(
        long contactId,
        string? newValue = null,
        ContactType? newType = null,
        string? newDescription = null
    )
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact == null)
            throw new InvalidOperationException($"Контакт с ID {contactId} не найден.");

        if (newValue != null)
            contact.UpdateValue(newValue);

        if (newType.HasValue)
            contact.UpdateType(newType.Value);

        contact.UpdateDescription(newDescription);
    }

    // Методы для управления EmploymentHistory
    public EmploymentHistory AddEmploymentHistory(
        string companyName,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null
    )
    {
        var employment = new EmploymentHistory(
            companyName,
            position,
            startDate,
            endDate,
            description
        );
        _employmentHistories.Add(employment);
        return employment;
    }

    public void RemoveEmploymentHistory(long employmentId)
    {
        var employment = _employmentHistories.FirstOrDefault(e => e.Id == employmentId);
        if (employment == null)
            throw new InvalidOperationException(
                $"История трудоустройства с ID {employmentId} не найдена."
            );

        employment.Delete();
    }

    public void UpdateEmploymentHistory(
        long employmentId,
        string? companyName = null,
        string? position = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? description = null
    )
    {
        var employment = _employmentHistories.FirstOrDefault(e => e.Id == employmentId);
        if (employment == null)
            throw new InvalidOperationException(
                $"История трудоустройства с ID {employmentId} не найдена."
            );

        employment.Update(companyName, position, startDate, endDate, description);
    }

    // Soft Delete
    public void Delete()
    {
        IsDeleted = true;

        // Каскадное удаление дочерних сущностей
        foreach (var contact in _contacts.Where(c => !c.IsDeleted))
            contact.Delete();

        foreach (var employment in _employmentHistories.Where(e => !e.IsDeleted))
            employment.Delete();

        FullName.Delete();
    }
}
