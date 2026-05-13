using Edvantix.Curriculum.Domain.Enums;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>
/// Учебный модуль — раздел курса, объединяющий связанные уроки.
/// Дочерняя сущность агрегата <see cref="Course"/>.
/// </summary>
public sealed class Module() : Entity
{
    private readonly List<Lesson> _lessons = [];

    /// <param name="courseId">Идентификатор курса-владельца.</param>
    /// <param name="position">Порядковый номер модуля в курсе (1-based).</param>
    /// <param name="name">Название модуля.</param>
    /// <param name="summary">Краткое описание модуля.</param>
    /// <param name="weeks">Рекомендуемая продолжительность в неделях.</param>
    public Module(Guid courseId, short position, string name, string? summary, short weeks)
        : this()
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор курса не может быть пустым.",
                nameof(courseId)
            );

        if (position <= 0)
            throw new ArgumentException("Позиция должна быть больше нуля.", nameof(position));

        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        if (weeks <= 0)
            throw new ArgumentException(
                "Продолжительность модуля должна быть больше нуля.",
                nameof(weeks)
            );

        Id = Guid.CreateVersion7();
        CourseId = courseId;
        Position = position;
        Name = name.Trim();
        Summary = summary?.Trim();
        Weeks = weeks;
    }

    /// <summary>Ссылка на курс-владелец.</summary>
    public Guid CourseId { get; private set; }

    /// <summary>Порядковый номер модуля в курсе (1-based).</summary>
    public short Position { get; private set; }

    /// <summary>Название модуля.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Краткое описание модуля.</summary>
    public string? Summary { get; private set; }

    /// <summary>Рекомендуемая продолжительность модуля в неделях.</summary>
    public short Weeks { get; private set; }

    /// <summary>Уроки модуля (в порядке позиций).</summary>
    public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();

    /// <summary>Добавляет урок в модуль.</summary>
    /// <returns>Созданный урок.</returns>
    internal Lesson AddLesson(string title, LessonType type, short minutes, string[] objectives)
    {
        var position = (short)(_lessons.Count + 1);
        var lesson = new Lesson(Id, position, title, type, minutes, objectives);
        _lessons.Add(lesson);
        return lesson;
    }

    /// <summary>Обновляет поля модуля.</summary>
    internal void Update(string name, string? summary, short weeks)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        if (weeks <= 0)
            throw new ArgumentException(
                "Продолжительность модуля должна быть больше нуля.",
                nameof(weeks)
            );

        Name = name.Trim();
        Summary = summary?.Trim();
        Weeks = weeks;
    }

    /// <summary>Устанавливает порядковый номер модуля (используется при переупорядочивании).</summary>
    internal void SetPosition(short position)
    {
        if (position <= 0)
            throw new ArgumentException("Позиция должна быть больше нуля.", nameof(position));

        Position = position;
    }

    /// <summary>
    /// Перемещает урок на новую позицию в рамках модуля и переиндексирует остальные уроки.
    /// </summary>
    internal void MoveLesson(Guid lessonId, short newPosition)
    {
        if (newPosition <= 0 || newPosition > _lessons.Count)
            throw new ArgumentOutOfRangeException(
                nameof(newPosition),
                $"Позиция должна быть в диапазоне 1..{_lessons.Count}."
            );

        var lesson =
            _lessons.FirstOrDefault(l => l.Id == lessonId)
            ?? throw new NotFoundException($"Урок {lessonId} не принадлежит модулю.");

        _lessons.Remove(lesson);
        _lessons.Insert(newPosition - 1, lesson);

        for (var i = 0; i < _lessons.Count; i++)
            _lessons[i].Move((short)(i + 1));
    }
}
