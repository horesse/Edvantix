using Edvantix.Curriculum.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>
/// Урок внутри учебного модуля.
/// Дочерняя сущность агрегата <see cref="Course"/> через <see cref="Module"/>.
/// </summary>
public sealed class Lesson() : Entity
{
    /// <param name="moduleId">Идентификатор модуля-владельца.</param>
    /// <param name="position">Порядковый номер урока в модуле (1-based).</param>
    /// <param name="title">Название урока.</param>
    /// <param name="type">Тип урока.</param>
    /// <param name="minutes">Продолжительность в минутах.</param>
    /// <param name="objectives">Цели урока (массив строк).</param>
    public Lesson(
        Guid moduleId,
        short position,
        string title,
        LessonType type,
        short minutes,
        string[] objectives
    )
        : this()
    {
        if (moduleId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор модуля не может быть пустым.",
                nameof(moduleId)
            );

        if (position <= 0)
            throw new ArgumentException("Позиция должна быть больше нуля.", nameof(position));

        Guard.Against.NullOrWhiteSpace(title, nameof(title));

        if (minutes <= 0)
            throw new ArgumentException(
                "Продолжительность урока должна быть больше нуля.",
                nameof(minutes)
            );

        Id = Guid.CreateVersion7();
        ModuleId = moduleId;
        Position = position;
        Title = title.Trim();
        Type = type;
        Status = LessonStatus.Draft;
        Minutes = minutes;
        Objectives = objectives ?? [];
    }

    /// <summary>Ссылка на модуль-владелец.</summary>
    public Guid ModuleId { get; private set; }

    /// <summary>Порядковый номер урока в модуле (1-based).</summary>
    public short Position { get; private set; }

    /// <summary>Название урока.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Тип занятия.</summary>
    public LessonType Type { get; private set; }

    /// <summary>Текущий статус урока.</summary>
    public LessonStatus Status { get; private set; }

    /// <summary>Продолжительность урока в минутах.</summary>
    public short Minutes { get; private set; }

    /// <summary>Цели урока.</summary>
    public string[] Objectives { get; private set; } = [];

    /// <summary>Публикует урок, переводя его в статус <see cref="LessonStatus.Published"/>.</summary>
    internal void Publish()
    {
        Status = LessonStatus.Published;
    }

    /// <summary>Перемещает урок на новую позицию.</summary>
    internal void Move(short newPosition)
    {
        if (newPosition <= 0)
            throw new ArgumentException("Позиция должна быть больше нуля.", nameof(newPosition));

        Position = newPosition;
    }

    /// <summary>Обновляет поля урока.</summary>
    internal void Update(string title, LessonType type, short minutes, string[] objectives)
    {
        Guard.Against.NullOrWhiteSpace(title, nameof(title));

        if (minutes <= 0)
            throw new ArgumentException(
                "Продолжительность урока должна быть больше нуля.",
                nameof(minutes)
            );

        Title = title.Trim();
        Type = type;
        Minutes = minutes;
        Objectives = objectives ?? [];
    }
}
