namespace Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;

/// <summary>Тип кабинета (помещения) организации.</summary>
public enum RoomType
{
    /// <summary>Учебный класс / аудитория.</summary>
    Classroom,

    /// <summary>Лаборатория.</summary>
    Lab,

    /// <summary>Переговорная / конференц-зал.</summary>
    Meeting,

    /// <summary>Онлайн (виртуальное пространство).</summary>
    Online,
}
