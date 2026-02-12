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

    public string? Avatar { get; private set; }

    public IReadOnlyCollection<UserContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();

    public IReadOnlyCollection<Education> Educations => _educations;

    public bool IsDeleted { get; set; }

    // Методы для управления основными полями профиля
    public void UpdateGender(Gender newGender)
    {
        Gender = newGender;
    }

    public void UpdateBirthDate(DateOnly newBirthDate)
    {
        if (newBirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException(
                "Дата рождения не может быть в будущем.",
                nameof(newBirthDate)
            );

        BirthDate = newBirthDate;
    }

    public void UpdateFullName(string firstName, string lastName, string? middleName = null)
    {
        FullName = new FullName(firstName, lastName, middleName);
    }

    public void UploadAvatar(string? avatarUrl)
    {
        Avatar = avatarUrl;
    }

    public UserContact CreateContact(ContactType type, string value, string? description = null)
    {
        return new UserContact(type, value, description);
    }

    public void AddContact(UserContact userContact)
    {
        _contacts.Add(userContact);
    }

    public void AddContacts(IEnumerable<UserContact> contacts)
    {
        _contacts.AddRange(contacts);
    }

    public void ClearContacts()
    {
        _contacts.Clear();
    }

    public void ReplaceContacts(IEnumerable<UserContact> contacts)
    {
        _contacts.Clear();
        _contacts.AddRange(contacts);
    }

    // Методы для управления историей трудоустройства
    public EmploymentHistory CreateEmploymentHistory(
        string workplace,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null
    )
    {
        return new EmploymentHistory(workplace, position, startDate, endDate, description);
    }

    public void AddEmploymentHistory(EmploymentHistory employmentHistory)
    {
        _employmentHistories.Add(employmentHistory);
    }

    public void AddEmploymentHistories(IEnumerable<EmploymentHistory> employmentHistories)
    {
        _employmentHistories.AddRange(employmentHistories);
    }

    public void ClearEmploymentHistories()
    {
        _employmentHistories.Clear();
    }

    public void ReplaceEmploymentHistories(IEnumerable<EmploymentHistory> employmentHistories)
    {
        _employmentHistories.Clear();
        _employmentHistories.AddRange(employmentHistories);
    }

    // Методы для управления образованием
    public Education CreateEducation(
        DateOnly dateStart,
        string institution,
        EducationLevel educationLevel,
        string? specialty = null,
        DateOnly? dateEnd = null
    )
    {
        return new Education(dateStart, institution, educationLevel, specialty, dateEnd);
    }

    public void AddEducation(Education education)
    {
        _educations.Add(education);
    }

    public void AddEducations(IEnumerable<Education> educations)
    {
        _educations.AddRange(educations);
    }

    public void ClearEducations()
    {
        _educations.Clear();
    }

    public void ReplaceEducations(IEnumerable<Education> educations)
    {
        _educations.Clear();
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
