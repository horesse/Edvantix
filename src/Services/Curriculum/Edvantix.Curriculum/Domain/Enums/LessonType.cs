namespace Edvantix.Curriculum.Domain.Enums;

/// <summary>Тип занятия внутри учебного модуля.</summary>
public enum LessonType
{
    /// <summary>Лекция — теоретический материал.</summary>
    Lecture,

    /// <summary>Практика — выполнение упражнений.</summary>
    Practice,

    /// <summary>Разговорное занятие.</summary>
    Speaking,

    /// <summary>Аудирование.</summary>
    Listening,

    /// <summary>Письмо.</summary>
    Writing,

    /// <summary>Тест / контрольная.</summary>
    Test,

    /// <summary>Повторение пройденного материала.</summary>
    Review,
}
