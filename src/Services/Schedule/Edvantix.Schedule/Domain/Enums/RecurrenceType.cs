namespace Edvantix.Schedule.Domain.Enums;

/// <summary>Тип рекуррентности расписания.</summary>
public enum RecurrenceType
{
    /// <summary>Еженедельно.</summary>
    Weekly,

    /// <summary>Раз в две недели.</summary>
    Biweekly,

    /// <summary>Произвольный (слоты вручную).</summary>
    Custom,
}
