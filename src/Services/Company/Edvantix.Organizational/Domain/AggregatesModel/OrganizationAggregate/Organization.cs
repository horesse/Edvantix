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

    public bool IsDeleted { get; set; }

    public void Delete()
    {
        IsDeleted = true;
    }
}
