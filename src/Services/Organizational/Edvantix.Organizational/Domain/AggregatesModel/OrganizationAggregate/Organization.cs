using Edvantix.Organizational.Domain.Enums;
using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Центральная сущность микросервиса Organization.
/// Представляет юридическое или структурное подразделение, участвующее в бизнес-процессах платформы.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>В одной организации может быть только один владелец (owner).</item>
///   <item>Нельзя удалить организацию, если к ней привязаны активные участники или договоры.</item>
///   <item>У организации всегда должен быть хотя бы один активный контакт, помеченный как Primary.</item>
/// </list>
/// </summary>
public sealed class Organization() : AuditableEntity, IAggregateRoot, ISoftDelete
{
    private readonly List<Contact> _contacts = [];

    /// <param name="fullLegalName">Полное юридическое наименование организации.</param>
    /// <param name="isLegalEntity">true — юридическое лицо, false — физическое лицо / ИП.</param>
    /// <param name="registrationDate">Дата официальной регистрации.</param>
    /// <param name="legalForm">Организационно-правовая форма.</param>
    /// <param name="countryId">Идентификатор страны регистрации (ISO 3166-1).</param>
    /// <param name="currencyId">Идентификатор базовой валюты (ISO 4217).</param>
    /// <param name="organizationType">Тип организации.</param>
    /// <param name="shortName">Сокращённое наименование для отображения в интерфейсе.</param>
    public Organization(
        string fullLegalName,
        bool isLegalEntity,
        DateOnly registrationDate,
        LegalForm legalForm,
        Guid countryId,
        Guid currencyId,
        OrganizationType organizationType,
        string? shortName = null
    )
        : this()
    {
        Id = Guid.CreateVersion7();
        Guard.Against.NullOrWhiteSpace(fullLegalName, nameof(fullLegalName));

        if (countryId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор страны не может быть пустым.",
                nameof(countryId)
            );
        if (currencyId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор валюты не может быть пустым.",
                nameof(currencyId)
            );

        FullLegalName = fullLegalName.Trim();
        ShortName = shortName?.Trim();
        IsLegalEntity = isLegalEntity;
        RegistrationDate = registrationDate;
        LegalForm = legalForm;
        CountryId = countryId;
        CurrencyId = currencyId;
        OrganizationType = organizationType;
        Status = OrganizationStatus.Active;
        IsDeleted = false;
    }

    /// <summary>Полное юридическое наименование организации.</summary>
    public string FullLegalName { get; private set; } = string.Empty;

    /// <summary>Сокращённое наименование для отображения в интерфейсе.</summary>
    public string? ShortName { get; private set; }

    /// <summary>true — юридическое лицо; false — физическое лицо / ИП.</summary>
    public bool IsLegalEntity { get; private set; }

    /// <summary>Дата официальной регистрации организации.</summary>
    public DateOnly RegistrationDate { get; private set; }

    /// <summary>Организационно-правовая форма.</summary>
    public LegalForm LegalForm { get; private set; }

    /// <summary>Идентификатор страны регистрации (ISO 3166-1).</summary>
    public Guid CountryId { get; private set; }

    /// <summary>Идентификатор базовой валюты (ISO 4217).</summary>
    public Guid CurrencyId { get; private set; }

    /// <summary>Тип организации.</summary>
    public OrganizationType OrganizationType { get; private set; }

    /// <summary>Текущий жизненный статус организации.</summary>
    public OrganizationStatus Status { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Контактные данные организации.</summary>
    public IReadOnlyList<Contact> Contacts => _contacts;

    /// <summary>
    /// Добавляет контакт в организацию.
    /// Если добавляется основной контакт, предыдущий основной снимается.
    /// </summary>
    public void AddContact(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        if (contact.IsPrimary)
        {
            foreach (var existing in _contacts.Where(c => c.IsPrimary))
            {
                existing.UnsetPrimary();
            }
        }

        _contacts.Add(contact);
    }

    /// <summary>
    /// Назначает контакт основным, снимая этот флаг с остальных.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если контакт не найден или удалён.</exception>
    public void SetPrimaryContact(Guid contactId)
    {
        var target =
            _contacts.FirstOrDefault(c => c.Id == contactId)
            ?? throw new InvalidOperationException(
                $"Активный контакт с идентификатором {contactId} не найден в организации."
            );

        foreach (var c in _contacts.Where(c => c.IsPrimary))
            c.UnsetPrimary();

        target.SetPrimary();
    }

    /// <summary>Обновляет основные реквизиты организации.</summary>
    public void Update(
        string fullLegalName,
        string? shortName,
        OrganizationType organizationType,
        LegalForm legalForm,
        DateOnly registrationDate
    )
    {
        Guard.Against.NullOrWhiteSpace(fullLegalName, nameof(fullLegalName));
        FullLegalName = fullLegalName.Trim();
        ShortName = shortName?.Trim();
        OrganizationType = organizationType;
        LegalForm = legalForm;
        RegistrationDate = registrationDate;
        LastModifiedAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(new OrganizationUpdatedDomainEvent(Id));
    }

    /// <summary>Обновляет основной контакт организации.</summary>
    /// <exception cref="InvalidOperationException">Если основной контакт не найден.</exception>
    public void UpdatePrimaryContact(ContactType contactType, string value, string description)
    {
        var primary =
            _contacts.FirstOrDefault(c => c.IsPrimary)
            ?? throw new InvalidOperationException("Основной контакт организации не найден.");
        primary.Update(value, description, contactType);
        LastModifiedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>
    /// Регистрирует доменное событие создания организации с назначением владельца.
    /// Вызывается однократно из application-слоя сразу после создания агрегата.
    /// </summary>
    public void InitializeOwnership(Guid ownerProfileId)
    {
        if (ownerProfileId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор профиля владельца не может быть пустым.",
                nameof(ownerProfileId)
            );

        RegisterDomainEvent(new OrganizationCreatedDomainEvent(Id, ownerProfileId));
    }

    /// <summary>Архивирует организацию.</summary>
    public void Archive() => Status = OrganizationStatus.Archived;

    /// <inheritdoc />
    /// <remarks>
    /// Проверка наличия активных участников (кроме owner) и договоров выполняется
    /// на уровне прикладного сервиса перед вызовом этого метода.
    /// </remarks>
    public void Delete()
    {
        IsDeleted = true;
        Status = OrganizationStatus.Deleted;

        RegisterDomainEvent(new OrganizationDeletedDomainEvent(Id));
    }
}
