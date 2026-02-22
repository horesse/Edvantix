using Edvantix.Constants.Other;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Корневой агрегат профиля пользователя. Управляет всеми дочерними сущностями
/// и обеспечивает инварианты домена.
/// </summary>
public sealed class Profile() : Entity, IAggregateRoot, ISoftDelete
{
    // Backing fields — EF Core обращается к ним напрямую через PropertyAccessMode.Field
    private readonly List<ProfileContact> _contacts = [];
    private readonly List<EmploymentHistory> _employmentHistories = [];
    private readonly List<Education> _educations = [];

    /// <summary>Создаёт новый профиль при регистрации пользователя.</summary>
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

        RegisterDomainEvent(new ProfileRegisteredEvent(accountId, login));
    }

    /// <summary>GUID аккаунта в Keycloak.</summary>
    public Guid AccountId { get; private set; }

    /// <summary>Preferred username из Keycloak.</summary>
    public string Login { get; private set; } = null!;

    /// <summary>Пол пользователя.</summary>
    public Gender Gender { get; private set; }

    /// <summary>Дата рождения.</summary>
    public DateOnly BirthDate { get; private set; }

    /// <summary>ФИО пользователя (owned entity, хранится в отдельной таблице).</summary>
    public FullName FullName { get; private set; } = null!;

    /// <summary>URN файла аватара в Azure Blob Storage. Null, если аватар не загружен.</summary>
    public string? AvatarUrl { get; private set; }

    public IReadOnlyCollection<ProfileContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();
    public IReadOnlyCollection<Education> Educations => _educations.AsReadOnly();

    public bool IsDeleted { get; set; }

    // ─── Scalar updates ───────────────────────────────────────────────────────

    /// <summary>Обновляет дату рождения. Пол устанавливается при регистрации и не может быть изменён.</summary>
    public void UpdatePersonalInfo(DateOnly birthDate) => BirthDate = birthDate;

    /// <summary>
    /// Обновляет ФИО, изменяя значения существующей сущности FullName.
    /// Не создаёт новую запись — обновляет текущую строку в БД.
    /// </summary>
    public void UpdateFullName(string firstName, string lastName, string? middleName = null) =>
        FullName.Update(firstName, lastName, middleName);

    /// <summary>Устанавливает URN аватара (null — удалить аватар).</summary>
    public void UploadAvatar(string? avatarUrl) => AvatarUrl = avatarUrl;

    // ─── Contact management ───────────────────────────────────────────────────

    /// <summary>
    /// Создаёт новый контакт. Не добавляет его в коллекцию —
    /// используйте <see cref="ReplaceContacts"/> для сохранения.
    /// </summary>
    public ProfileContact CreateContact(
        ContactType type,
        string value,
        string? description = null
    ) => new(type, value, description);

    /// <summary>
    /// Заменяет все контакты профиля новым набором.
    /// Старые записи удаляются через каскадное удаление EF Core.
    /// </summary>
    public void ReplaceContacts(IEnumerable<ProfileContact> contacts)
    {
        _contacts.Clear();
        _contacts.AddRange(contacts);
    }

    // ─── Employment history management ────────────────────────────────────────

    /// <summary>
    /// Создаёт новую запись об опыте работы. Не добавляет её в коллекцию —
    /// используйте <see cref="ReplaceEmploymentHistories"/> для сохранения.
    /// </summary>
    public EmploymentHistory CreateEmploymentHistory(
        string workplace,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null
    ) => new(workplace, position, startDate, endDate, description);

    /// <summary>
    /// Заменяет всю историю занятости новым набором.
    /// Старые записи удаляются через каскадное удаление EF Core.
    /// </summary>
    public void ReplaceEmploymentHistories(IEnumerable<EmploymentHistory> employmentHistories)
    {
        _employmentHistories.Clear();
        _employmentHistories.AddRange(employmentHistories);
    }

    // ─── Education management ─────────────────────────────────────────────────

    /// <summary>
    /// Создаёт новую запись об образовании. Не добавляет её в коллекцию —
    /// используйте <see cref="ReplaceEducations"/> для сохранения.
    /// </summary>
    public Education CreateEducation(
        DateOnly dateStart,
        string institution,
        EducationLevel educationLevel,
        string? specialty = null,
        DateOnly? dateEnd = null
    ) => new(dateStart, institution, educationLevel, specialty, dateEnd);

    /// <summary>
    /// Заменяет весь список образования новым набором.
    /// Старые записи удаляются через каскадное удаление EF Core.
    /// </summary>
    public void ReplaceEducations(IEnumerable<Education> educations)
    {
        _educations.Clear();
        _educations.AddRange(educations);
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    /// <summary>Помечает профиль как удалённый (мягкое удаление).</summary>
    public void Delete() => IsDeleted = true;
}
