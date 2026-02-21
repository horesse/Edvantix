using Edvantix.Constants.Other;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class Profile() : Entity, IAggregateRoot, ISoftDelete
{
    private readonly List<ProfileContact> _contacts = [];
    private readonly List<EmploymentHistory> _employmentHistories = [];
    private readonly List<Education> _educations = [];

    public Profile(
        Guid accountId,
        string login,
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

        ArgumentException.ThrowIfNullOrWhiteSpace(login, nameof(login));

        AccountId = accountId;
        Login = login;
        Gender = gender;
        BirthDate = birthDate;
        FullName = new FullName(firstName, lastName, middleName);
    }

    public Guid AccountId { get; private set; }
    public string Login { get; private set; } = null!;
    public Gender Gender { get; private set; }
    public DateOnly BirthDate { get; private set; }

    public FullName FullName { get; private set; } = null!;

    public string? AvatarUrl { get; private set; }

    public IReadOnlyCollection<ProfileContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();

    public IReadOnlyCollection<Education> Educations => _educations;

    public bool IsDeleted { get; set; }

    public void UpdateFullName(string firstName, string lastName, string? middleName = null)
    {
        FullName = new FullName(firstName, lastName, middleName);
    }

    public void UploadAvatar(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }

    public ProfileContact CreateContact(ContactType type, string value, string? description = null)
    {
        return new ProfileContact(type, value, description);
    }

    public void AddContact(ProfileContact profileContact)
    {
        _contacts.Add(profileContact);
    }

    public void AddContacts(IEnumerable<ProfileContact> contacts)
    {
        _contacts.AddRange(contacts);
    }

    public void ClearContacts()
    {
        _contacts.Clear();
    }

    public void ReplaceContacts(IEnumerable<ProfileContact> contacts)
    {
        _contacts.Clear();
        _contacts.AddRange(contacts);
    }

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

    public void Delete()
    {
        IsDeleted = true;
    }
}
