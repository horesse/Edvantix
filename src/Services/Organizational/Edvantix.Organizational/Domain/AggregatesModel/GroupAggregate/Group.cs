using Edvantix.Organizational.Domain.Events;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Учебная группа — логическое объединение участников внутри одной организации.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item>Архивированные группы нельзя редактировать.</item>
///   <item><see cref="EndDate"/> является обязательным — группа всегда имеет плановую дату завершения.</item>
///   <item>При формате <see cref="GroupFormat.Offline"/> или <see cref="GroupFormat.Mixed"/> должен быть указан <see cref="RoomId"/>.</item>
///   <item>При формате <see cref="GroupFormat.Online"/> или <see cref="GroupFormat.Mixed"/> должна быть указана <see cref="Platform"/>.</item>
///   <item><see cref="Capacity"/> — от 1 до 50 участников.</item>
///   <item><see cref="Code"/> уникален в рамках организации.</item>
/// </list>
/// </summary>
public sealed class Group() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    private const int MinCapacity = 1;
    private const int MaxCapacity = 50;

    private readonly List<GroupMember> _members = [];

    /// <param name="organizationId">Идентификатор организации-владельца группы.</param>
    /// <param name="code">Уникальный код группы в рамках организации (напр. <c>EN-B1-12</c>).</param>
    /// <param name="name">Наименование группы.</param>
    /// <param name="description">Описание группы.</param>
    /// <param name="level">Уровень сложности.</param>
    /// <param name="courseId">Идентификатор курса из Curriculum-сервиса (логическая FK).</param>
    /// <param name="teacherMemberId">Идентификатор преподавателя — <c>OrganizationMember.Id</c>.</param>
    /// <param name="format">Формат занятий.</param>
    /// <param name="roomId">Кабинет; обязателен при <c>Offline</c>/<c>Mixed</c>.</param>
    /// <param name="platform">Онлайн-платформа; обязательна при <c>Online</c>/<c>Mixed</c>.</param>
    /// <param name="capacity">Максимальное число участников (1–50).</param>
    /// <param name="startDate">Дата начала работы группы.</param>
    /// <param name="endDate">Дата окончания работы группы.</param>
    public Group(
        Guid organizationId,
        GroupCode code,
        string name,
        string description,
        GroupLevel level,
        Guid courseId,
        Guid teacherMemberId,
        GroupFormat format,
        Guid? roomId,
        OnlinePlatform? platform,
        int capacity,
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

        if (courseId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор курса не может быть пустым.",
                nameof(courseId)
            );

        if (teacherMemberId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор преподавателя не может быть пустым.",
                nameof(teacherMemberId)
            );

        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));
        ArgumentNullException.ThrowIfNull(code);

        ValidateFormatRoomPlatform(format, roomId, platform);
        ValidateCapacity(capacity);

        if (endDate <= startDate)
            throw new ArgumentException(
                "Дата окончания группы должна быть позже даты начала.",
                nameof(endDate)
            );

        OrganizationId = organizationId;
        Code = code;
        Name = name.Trim();
        Description = description.Trim();
        Level = level;
        CourseId = courseId;
        TeacherMemberId = teacherMemberId;
        Format = format;
        RoomId = roomId;
        Platform = platform;
        Capacity = capacity;
        StartDate = startDate;
        EndDate = endDate;
        Status = GroupStatus.Recruiting;
        IsDeleted = false;

        RegisterDomainEvent(new GroupCreatedDomainEvent(Id, organizationId, startDate));
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Уникальный код группы в рамках организации.</summary>
    public GroupCode Code { get; private set; } = GroupCode.From("NEW");

    /// <summary>Наименование группы.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Описание группы.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Уровень сложности / целевая аудитория.</summary>
    public GroupLevel Level { get; private set; }

    /// <summary>
    /// Идентификатор курса в Curriculum-сервисе.
    /// Хранится как логическая FK — без constraint на уровне БД (cross-database).
    /// </summary>
    public Guid CourseId { get; private set; }

    /// <summary>
    /// Идентификатор преподавателя группы — <c>OrganizationMember.Id</c>.
    /// </summary>
    public Guid TeacherMemberId { get; private set; }

    /// <summary>Формат проведения занятий.</summary>
    public GroupFormat Format { get; private set; }

    /// <summary>
    /// Идентификатор кабинета (<c>Room.Id</c>).
    /// Обязателен при <see cref="GroupFormat.Offline"/> и <see cref="GroupFormat.Mixed"/>.
    /// </summary>
    public Guid? RoomId { get; private set; }

    /// <summary>
    /// Онлайн-платформа для занятий.
    /// Обязательна при <see cref="GroupFormat.Online"/> и <see cref="GroupFormat.Mixed"/>.
    /// </summary>
    public OnlinePlatform? Platform { get; private set; }

    /// <summary>Максимальное число участников группы (1–50).</summary>
    public int Capacity { get; private set; }

    /// <summary>Дата начала работы группы.</summary>
    public DateOnly StartDate { get; private set; }

    /// <summary>
    /// Дата окончания работы группы. NOT NULL — группа всегда имеет плановую дату завершения.
    /// </summary>
    public DateOnly EndDate { get; private set; }

    /// <summary>Текущий статус группы.</summary>
    public GroupStatus Status { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Участники группы.</summary>
    public IReadOnlyList<GroupMember> Members => _members;

    /// <summary>
    /// Обновляет основные данные группы.
    /// Архивированные группы не могут быть изменены.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если группа архивирована.</exception>
    public void Update(
        string name,
        string description,
        GroupLevel level,
        Guid courseId,
        Guid teacherMemberId,
        GroupFormat format,
        Guid? roomId,
        OnlinePlatform? platform,
        int capacity,
        DateOnly endDate
    )
    {
        EnsureNotArchived();
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(description, nameof(description));

        if (courseId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор курса не может быть пустым.",
                nameof(courseId)
            );

        if (teacherMemberId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор преподавателя не может быть пустым.",
                nameof(teacherMemberId)
            );

        ValidateFormatRoomPlatform(format, roomId, platform);
        ValidateCapacity(capacity);

        if (endDate <= StartDate)
            throw new ArgumentException(
                "Дата окончания группы должна быть позже даты начала.",
                nameof(endDate)
            );

        Name = name.Trim();
        Description = description.Trim();
        Level = level;
        CourseId = courseId;
        TeacherMemberId = teacherMemberId;
        Format = format;
        RoomId = roomId;
        Platform = platform;
        Capacity = capacity;
        EndDate = endDate;
    }

    /// <summary>Изменяет статус группы.</summary>
    /// <param name="newStatus">Новый статус.</param>
    /// <exception cref="InvalidOperationException">Если группа архивирована.</exception>
    public void ChangeStatus(GroupStatus newStatus)
    {
        EnsureNotArchived();
        Status = newStatus;
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
    public void Archive() => Status = GroupStatus.Archived;

    /// <summary>
    /// Восстанавливает архивированную группу, возвращая её в статус <see cref="GroupStatus.Recruiting"/>.
    /// </summary>
    public void Restore() => Status = GroupStatus.Recruiting;

    /// <inheritdoc />
    public void Delete()
    {
        IsDeleted = true;
        Status = GroupStatus.Archived;
    }

    private void EnsureNotArchived()
    {
        if (Status == GroupStatus.Archived)
            throw new InvalidOperationException("Архивированную группу нельзя редактировать.");
    }

    private static void ValidateFormatRoomPlatform(
        GroupFormat format,
        Guid? roomId,
        OnlinePlatform? platform
    )
    {
        // Очный/смешанный формат требует указания кабинета
        if (format is GroupFormat.Offline or GroupFormat.Mixed && roomId is null)
            throw new ArgumentException(
                $"При формате {format} необходимо указать кабинет.",
                nameof(roomId)
            );

        // Онлайн/смешанный формат требует указания платформы
        if (format is GroupFormat.Online or GroupFormat.Mixed && platform is null)
            throw new ArgumentException(
                $"При формате {format} необходимо указать онлайн-платформу.",
                nameof(platform)
            );
    }

    private static void ValidateCapacity(int capacity)
    {
        if (capacity < MinCapacity || capacity > MaxCapacity)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                $"Вместимость группы должна быть от {MinCapacity} до {MaxCapacity} участников."
            );
    }
}
