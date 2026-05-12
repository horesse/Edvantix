using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;

/// <summary>
/// Цель курса — чему научится студент по завершению.
/// Дочерняя сущность агрегата <see cref="Course"/>.
/// </summary>
public sealed class CourseGoal() : Entity
{
    /// <param name="courseId">Идентификатор курса-владельца.</param>
    /// <param name="position">Порядковый номер (1-based).</param>
    /// <param name="text">Текст цели (до 256 символов).</param>
    public CourseGoal(Guid courseId, short position, string text)
        : this()
    {
        if (courseId == Guid.Empty)
            throw new ArgumentException(
                "Идентификатор курса не может быть пустым.",
                nameof(courseId)
            );

        if (position <= 0)
            throw new ArgumentException("Позиция должна быть больше нуля.", nameof(position));

        Guard.Against.NullOrWhiteSpace(text, nameof(text));

        Id = Guid.CreateVersion7();
        CourseId = courseId;
        Position = position;
        Text = text.Trim();
    }

    /// <summary>Ссылка на курс-владелец.</summary>
    public Guid CourseId { get; private set; }

    /// <summary>Порядковый номер цели (1-based).</summary>
    public short Position { get; private set; }

    /// <summary>Текст цели.</summary>
    public string Text { get; private set; } = string.Empty;

    /// <summary>Обновляет текст цели.</summary>
    public void Update(string text)
    {
        Guard.Against.NullOrWhiteSpace(text, nameof(text));
        Text = text.Trim();
    }
}
