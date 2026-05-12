using Edvantix.Curriculum.Domain.Enums;
using Edvantix.Curriculum.Domain.Events;
using Edvantix.SharedKernel.Helpers;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>
/// Учебный курс — корневой агрегат каталога программ обучения.
/// <para>Бизнес-правила:</para>
/// <list type="bullet">
///   <item><see cref="Code"/> уникален в рамках организации (формат <c>EN-GEN-B1</c>).</item>
///   <item>Только <see cref="CourseStatus.Active"/> курсы доступны для привязки к группам.</item>
///   <item>Архивированный курс нельзя редактировать.</item>
/// </list>
/// </summary>
public sealed class Course() : AuditableEntity, IAggregateRoot, ISoftDelete, ITenanted
{
    private readonly List<CourseGoal> _goals = [];
    private readonly List<Module> _modules = [];

    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="code">Уникальный код курса (напр. <c>EN-GEN-B1</c>).</param>
    /// <param name="name">Наименование курса.</param>
    /// <param name="subject">Предметная область.</param>
    /// <param name="level">Уровень сложности (A1, B2, Kids и т.д.).</param>
    /// <param name="durationWeeks">Плановая продолжительность в неделях.</param>
    /// <param name="ownerMemberId">Идентификатор <c>OrganizationMember</c> — автор курса.</param>
    /// <param name="description">Описание курса.</param>
    public Course(
        Guid organizationId,
        string code,
        string name,
        CourseSubject subject,
        string level,
        short durationWeeks,
        Guid ownerMemberId,
        string? description = null
    )
        : this()
    {
        Id = Guid.CreateVersion7();

        if (organizationId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор организации не может быть пустым.",
                nameof(organizationId)
            );

        if (ownerMemberId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор владельца не может быть пустым.",
                nameof(ownerMemberId)
            );

        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(level, nameof(level));

        if (durationWeeks <= 0)
            throw new ArgumentException(
                "Продолжительность курса должна быть больше нуля.",
                nameof(durationWeeks)
            );

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Subject = subject;
        Level = level.Trim();
        DurationWeeks = durationWeeks;
        OwnerMemberId = ownerMemberId;
        Description = description?.Trim();
        Status = CourseStatus.Draft;
        LastModifiedAt = CreatedAt;

        RegisterDomainEvent(new CourseCreatedDomainEvent(Id, OrganizationId, OwnerMemberId));
    }

    /// <inheritdoc />
    public Guid OrganizationId { get; private set; }

    /// <summary>Уникальный код курса в рамках организации.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Наименование курса.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Предметная область курса.</summary>
    public CourseSubject Subject { get; private set; }

    /// <summary>Уровень сложности (A1, A2, B1, B2, C1, Kids, Teen, Any).</summary>
    public string Level { get; private set; } = string.Empty;

    /// <summary>Плановая продолжительность курса в неделях.</summary>
    public short DurationWeeks { get; private set; }

    /// <summary>Идентификатор <c>OrganizationMember</c> — автора курса.</summary>
    public Guid OwnerMemberId { get; private set; }

    /// <summary>Описание курса.</summary>
    public string? Description { get; private set; }

    /// <summary>Аббревиатура для UI-плейсхолдера обложки (до 4 символов).</summary>
    public string? CoverInitials { get; private set; }

    /// <summary>Текущий статус курса.</summary>
    public CourseStatus Status { get; private set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <summary>Цели курса.</summary>
    public IReadOnlyList<CourseGoal> Goals => _goals.AsReadOnly();

    /// <summary>Модули курса (в порядке позиций).</summary>
    public IReadOnlyList<Module> Modules => _modules.AsReadOnly();

    /// <summary>Кэшированное суммарное количество уроков.</summary>
    public int TotalLessons => _modules.Sum(m => m.Lessons.Count);

    /// <summary>
    /// Публикует курс, переводя его в статус <see cref="CourseStatus.Active"/>.
    /// </summary>
    public void Publish()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя опубликовать архивированный курс.");

        Status = CourseStatus.Active;
        LastModifiedAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(new CoursePublishedDomainEvent(Id, OrganizationId));
    }

    /// <summary>
    /// Архивирует курс, переводя его в статус <see cref="CourseStatus.Archived"/>.
    /// Архивированный курс нельзя использовать в новых группах.
    /// </summary>
    public void Archive()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Курс уже удалён.");

        Status = CourseStatus.Archived;
        LastModifiedAt = DateTimeHelper.UtcNow();

        RegisterDomainEvent(new CourseArchivedDomainEvent(Id, OrganizationId));
    }

    /// <inheritdoc />
    public void Delete()
    {
        IsDeleted = true;
        Status = CourseStatus.Archived;
        LastModifiedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>Обновляет основные поля курса.</summary>
    public void Update(
        string name,
        string? description,
        string level,
        short durationWeeks,
        string? coverInitials = null
    )
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(level, nameof(level));

        if (durationWeeks <= 0)
            throw new ArgumentException(
                "Продолжительность курса должна быть больше нуля.",
                nameof(durationWeeks)
            );

        Name = name.Trim();
        Description = description?.Trim();
        Level = level.Trim();
        DurationWeeks = durationWeeks;
        CoverInitials = coverInitials?.Trim();
        LastModifiedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>
    /// Добавляет цель курса.
    /// </summary>
    /// <returns>Созданная цель.</returns>
    public CourseGoal AddGoal(string text)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        var position = (short)(_goals.Count + 1);
        var goal = new CourseGoal(Id, position, text);
        _goals.Add(goal);
        LastModifiedAt = DateTimeHelper.UtcNow();
        return goal;
    }

    /// <summary>
    /// Добавляет модуль в курс.
    /// </summary>
    /// <returns>Созданный модуль.</returns>
    public Module AddModule(string name, string? summary, short weeks)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        var position = (short)(_modules.Count + 1);
        var module = new Module(Id, position, name, summary, weeks);
        _modules.Add(module);
        LastModifiedAt = DateTimeHelper.UtcNow();
        return module;
    }

    /// <summary>
    /// Переупорядочивает модули курса.
    /// </summary>
    /// <param name="orderedModuleIds">Идентификаторы модулей в новом порядке.</param>
    public void ReorderModules(IReadOnlyList<Guid> orderedModuleIds)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        if (orderedModuleIds.Count != _modules.Count)
            throw new ArgumentException(
                "Количество модулей в запросе не совпадает с количеством модулей курса.",
                nameof(orderedModuleIds)
            );

        for (var i = 0; i < orderedModuleIds.Count; i++)
        {
            var id = orderedModuleIds[i];
            var module =
                _modules.FirstOrDefault(m => m.Id == id)
                ?? throw new ArgumentException($"Модуль {id} не принадлежит данному курсу.");
            module.SetPosition((short)(i + 1));
        }

        LastModifiedAt = DateTimeHelper.UtcNow();
    }

    /// <summary>
    /// Добавляет урок в указанный модуль.
    /// </summary>
    /// <returns>Созданный урок.</returns>
    public Lesson AddLesson(
        Guid moduleId,
        string title,
        LessonType type,
        short minutes,
        string[] objectives
    )
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        var module =
            _modules.FirstOrDefault(m => m.Id == moduleId)
            ?? throw new NotFoundException($"Модуль {moduleId} не принадлежит данному курсу.");

        var lesson = module.AddLesson(title, type, minutes, objectives);
        LastModifiedAt = DateTimeHelper.UtcNow();
        return lesson;
    }

    /// <summary>
    /// Публикует урок в указанном модуле.
    /// </summary>
    public void PublishLesson(Guid lessonId)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя редактировать архивированный курс.");

        foreach (var module in _modules)
        {
            var lesson = module.Lessons.FirstOrDefault(l => l.Id == lessonId);

            if (lesson is null)
                continue;

            lesson.Publish();
            LastModifiedAt = DateTimeHelper.UtcNow();
            RegisterDomainEvent(new LessonPublishedDomainEvent(Id, module.Id, lessonId));
            return;
        }

        throw NotFoundException.For<Lesson>(lessonId);
    }
}
