namespace Edvantix.Organizational.Features.Rooms;

/// <summary>DTO кабинета организации.</summary>
/// <param name="Id">Идентификатор кабинета.</param>
/// <param name="OrganizationId">Идентификатор организации.</param>
/// <param name="Label">Метка кабинета, например «Каб. 204».</param>
/// <param name="Floor">Номер этажа.</param>
/// <param name="Seats">Количество посадочных мест.</param>
public sealed record RoomDto(Guid Id, Guid OrganizationId, string Label, short Floor, short Seats)
{
    /// <summary>
    /// Кабинет подходит по вместимости, но с минимальным запасом (менее 30% свободных мест).
    /// Используется компонентом <c>RoomSelect</c> для визуальной индикации.
    /// </summary>
    public bool FitsTight { get; init; }
}
