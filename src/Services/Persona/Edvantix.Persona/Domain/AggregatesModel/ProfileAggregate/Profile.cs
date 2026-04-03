using Edvantix.Constants.Other;
using Edvantix.Persona.Domain.Events;

namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Корневой агрегат профиля пользователя. Управляет всеми дочерними сущностями
/// и обеспечивает инварианты домена.
/// </summary>
public sealed class Profile() : Entity, IAggregateRoot, ISoftDelete
{
    /// <summary>Максимальное количество навыков на один профиль.</summary>
    public const int MaxSkillsCount = 20;

    // Backing fields — EF Core обращается к ним напрямую через PropertyAccessMode.Field
    private readonly List<ProfileContact> _contacts = [];
    private readonly List<EmploymentHistory> _employmentHistories = [];
    private readonly List<Education> _educations = [];
    private readonly List<ProfileSkill> _skills = [];

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

    /// <summary>Краткое описание "О себе". Максимум 600 символов.</summary>
    public string? Bio { get; private set; }

    public IReadOnlyCollection<ProfileContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<EmploymentHistory> EmploymentHistories =>
        _employmentHistories.AsReadOnly();
    public IReadOnlyCollection<Education> Educations => _educations.AsReadOnly();
    public IReadOnlyCollection<ProfileSkill> Skills => _skills.AsReadOnly();

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

    /// <summary>
    /// Устанавливает URN аватара при загрузке или замене.
    /// Если аватар уже был установлен, публикует <see cref="AvatarDeletedDomainEvent"/>
    /// для удаления старого файла из хранилища после сохранения.
    /// </summary>
    public void UploadAvatar(string avatarUrl)
    {
        if (AvatarUrl is not null)
            RegisterDomainEvent(new AvatarDeletedDomainEvent(AvatarUrl));

        AvatarUrl = avatarUrl;
    }

    /// <summary>
    /// Удаляет аватар профиля и публикует <see cref="AvatarDeletedDomainEvent"/>
    /// для последующего удаления файла из хранилища.
    /// Не выполняет никаких действий, если аватар не был установлен.
    /// </summary>
    public void DeleteAvatar()
    {
        if (AvatarUrl is null)
            return;

        RegisterDomainEvent(new AvatarDeletedDomainEvent(AvatarUrl));
        AvatarUrl = null;
    }

    /// <summary>Обновляет описание "О себе". Максимум <see cref="DataSchemaLength.SuperLarge"/> символов.</summary>
    public void UpdateBio(string? bio) => Bio = bio;

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

    // ─── Skills management ────────────────────────────────────────────────────

    /// <summary>
    /// Создаёт запись о связи профиля с навыком. Не добавляет её в коллекцию —
    /// используйте <see cref="ReplaceSkills"/> для сохранения.
    /// </summary>
    public ProfileSkill CreateSkill(Guid skillId) => new(skillId);

    /// <summary>
    /// Заменяет все навыки профиля новым набором (максимум <see cref="MaxSkillsCount"/>).
    /// Для каждого удалённого навыка публикует <see cref="SkillRemovedFromProfileDomainEvent"/>,
    /// чтобы после сохранения очистить неиспользуемые записи в глобальном каталоге.
    /// </summary>
    public void ReplaceSkills(IReadOnlyList<Guid> skillIds)
    {
        if (skillIds.Count > MaxSkillsCount)
            throw new InvalidOperationException(
                $"Профиль не может содержать более {MaxSkillsCount} навыков."
            );

        var removedSkillIds = _skills.Select(s => s.SkillId).Except(skillIds).ToList();

        foreach (var skillId in removedSkillIds)
            RegisterDomainEvent(new SkillRemovedFromProfileDomainEvent(Id, skillId));

        _skills.Clear();
        _skills.AddRange(skillIds.Select(id => new ProfileSkill(id)));
    }

    // ─── Lifecycle ────────────────────────────────────────────────────────────

    /// <summary>Помечает профиль как удалённый (мягкое удаление).</summary>
    public void Delete() => IsDeleted = true;

    /// <summary>Признак блокировки аккаунта.</summary>
    public bool IsBlocked { get; private set; }

    /// <summary>Дата и время последнего входа пользователя.</summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>Блокирует профиль администратором.</summary>
    public void Block() => IsBlocked = true;

    /// <summary>Снимает блокировку с профиля.</summary>
    public void Unblock() => IsBlocked = false;

    /// <summary>Обновляет время последнего входа пользователя.</summary>
    public void RecordLastLogin() => LastLoginAt = DateTime.UtcNow;
}
