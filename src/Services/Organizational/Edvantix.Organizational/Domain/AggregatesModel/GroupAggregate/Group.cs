using Edvantix.Organizational.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Учебная группа — логическое объединение пользователей внутри одной организации.
/// Группа всегда принадлежит одной организации, имеет чёткие временные рамки и статус
/// жизненного цикла.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>Архивированные группы нельзя редактировать.</item>
///   <item>Завершённая группа (<see cref="EndDate"/> в прошлом) архивируется автоматически.</item>
///   <item><see cref="EndDate"/> является обязательным (группа всегда имеет плановую дату завершения).</item>
/// </list>
/// </summary>
public sealed class Group() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private readonly List<GroupMember> _members = [];

    /// <param name="organizationId">Идентификатор организации-владельца группы.</param>
    /// <param name="name">Наименование группы.</param>
    /// <param name="description">Описание группы, её назначения или курса.</param>
    /// <param name="startDate">Дата начала работы группы.</param>
    /// <param name="endDate">Дата окончания работы группы (обязательна).</param>
    public Group(
        Guid organizationId,
        string name,
        string description,
        DateOnly startDate,
        DateOnly endDate
    )
        : this()
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));

        if (endDate <= startDate)
        {
            throw new ArgumentException(
                "Дата окончания группы должна быть позже даты начала.",
                nameof(endDate)
            );
        }

        OrganizationId = organizationId;
        Name = name.Trim();
        Description = description.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = OrganizationStatus.Active;
        IsDeleted = false;
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Наименование группы.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Описание группы, её назначения или курса.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Дата начала работы группы.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// Дата окончания работы группы. NOT NULL — группа всегда имеет плановую дату завершения.
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>Текущий статус группы.</summary>
    public OrganizationStatus Status { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Участники группы.</summary>
    public IReadOnlyList<GroupMember> Members => _members;

    /// <summary>
    /// Обновляет данные группы.
    /// Архивированные группы не могут быть изменены.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если группа архивирована.</exception>
    public void Update(string name, string description, DateOnly endDate)
    {
        EnsureNotArchived();
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));

        if (endDate <= StartDate)
        {
            throw new ArgumentException(
                "Дата окончания группы должна быть позже даты начала.",
                nameof(endDate)
            );
        }

        Name = name.Trim();
        Description = description.Trim();
        EndDate = endDate;
    }

    /// <summary>
    /// Добавляет участника в группу.
    /// Архивированные группы не принимают новых участников.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если группа архивирована.</exception>
    public void AddMember(GroupMember member)
    {
        EnsureNotArchived();
        ArgumentNullException.ThrowIfNull(member);
        _members.Add(member);
    }

    /// <summary>Архивирует группу. Архивированные группы нельзя редактировать.</summary>
    public void Archive() => Status = OrganizationStatus.Archived;

    /// <inheritdoc />
    public void Delete()
    {
        IsDeleted = true;
        Status = OrganizationStatus.Deleted;
    }

    private void EnsureNotArchived()
    {
        if (Status == OrganizationStatus.Archived)
        {
            throw new InvalidOperationException("Архивированную группу нельзя редактировать.");
        }
    }
}
