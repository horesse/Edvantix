using Edvantix.Curriculum.Domain.Enums;
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
public sealed class Course() : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
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
            throw new ArgumentException("Идентификатор организации не может быть пустым.", nameof(organizationId));

        if (ownerMemberId == Guid.Empty)
            throw new ArgumentException("Идентификатор владельца не может быть пустым.", nameof(ownerMemberId));

        Guard.Against.NullOrWhiteSpace(code, nameof(code));
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.NullOrWhiteSpace(level, nameof(level));

        if (durationWeeks <= 0)
            throw new ArgumentException("Продолжительность курса должна быть больше нуля.", nameof(durationWeeks));

        OrganizationId = organizationId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Subject = subject;
        Level = level.Trim();
        DurationWeeks = durationWeeks;
        OwnerMemberId = ownerMemberId;
        Description = description?.Trim();
        Status = CourseStatus.Draft;
        CreatedAt = DateTimeHelper.UtcNow();
        UpdatedAt = CreatedAt;
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

    /// <summary>Дата создания.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата последнего обновления.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Публикует курс, переводя его в статус <see cref="CourseStatus.Active"/>.
    /// </summary>
    public void Publish()
    {
        if (IsDeleted)
            throw new InvalidOperationException("Нельзя опубликовать архивированный курс.");

        Status = CourseStatus.Active;
        UpdatedAt = DateTimeHelper.UtcNow();
    }

    /// <inheritdoc />
    public void Delete()
    {
        IsDeleted = true;
        Status = CourseStatus.Archived;
        UpdatedAt = DateTimeHelper.UtcNow();
    }
}
