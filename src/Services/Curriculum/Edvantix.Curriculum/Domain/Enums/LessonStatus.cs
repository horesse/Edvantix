namespace Edvantix.Curriculum.Domain.Enums;

/// <summary>Статус урока.</summary>
public enum LessonStatus
{
    /// <summary>Черновик — урок в разработке.</summary>
    Draft,

    /// <summary>Опубликован — доступен для проведения.</summary>
    Published,

    /// <summary>Запланирован — подготовлен, но ещё не активен.</summary>
    Planned,
}
