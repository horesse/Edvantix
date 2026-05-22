namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

/// <summary>
/// Визуальный тон статуса студента — определяет цветовую индикацию в UI.
/// </summary>
public enum StudentStatusTone
{
    /// <summary>Активный / положительный.</summary>
    Active,

    /// <summary>Предупреждение / внимание.</summary>
    Warning,

    /// <summary>Нейтральный.</summary>
    Neutral,

    /// <summary>Неактивный / негативный.</summary>
    Inactive,
}
