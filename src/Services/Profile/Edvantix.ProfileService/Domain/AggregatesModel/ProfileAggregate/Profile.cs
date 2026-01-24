using Edvantix.Chassis.EF.Attributes;
using Edvantix.Constants.Other;
using Edvantix.ProfileService.Domain.AggregatesModel.ContactAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;
using Edvantix.ProfileService.Domain.AggregatesModel.FullNameAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Domain.AggregatesModel.ProfileAggregate;

public sealed class Profile() : Entity<long>, IAggregateRoot, ISoftDelete
{
    private readonly List<UserContact> _contacts = [];
    private readonly List<EmploymentHistory> _employmentHistories = [];
    private readonly List<Education> _educations = [];

    public Profile(
        Guid accountId,
        Gender gender,
        DateOnly birthDate,
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
        BirthDate = birthDate;
        FullName = new FullName(firstName, lastName, middleName);
    }

    public Guid AccountId { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly BirthDate { get; private set; }

    [Include]
    public FullName FullName { get; private set; } = null!;

    public IReadOnlyCollection<UserContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();

    public IReadOnlyCollection<Education> Educations => _educations;

    public bool IsDeleted { get; set; }

    // Методы для управления Gender
    public void UpdateGender(Gender newGender)
    {
        Gender = newGender;
    }

    public void AddContact(UserContact userContact)
    {
        _contacts.Add(userContact);
    }

    public void AddContacts(IEnumerable<UserContact> contacts)
    {
        _contacts.AddRange(contacts);
    }

    public void AddEmploymentHistory(EmploymentHistory employmentHistory)
    {
        _employmentHistories.Add(employmentHistory);
    }

    public void AddEmploymentHistories(IEnumerable<EmploymentHistory> employmentHistories)
    {
        _employmentHistories.AddRange(employmentHistories);
    }

    public void AddEducation(Education education)
    {
        _educations.Add(education);
    }

    public void AddEducations(IEnumerable<Education> educations)
    {
        _educations.AddRange(educations);
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
