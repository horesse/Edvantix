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
